using AutoMapper;
using famenova.Domain.Common;
using famenova.Domain.Entities;
using famenova.Domain.Enums;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Famenova.Application.Specifications.param;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.Pay;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Famenova.Application.Services
{

    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;
        private readonly IOrderConfirmationService _confirmationService;
        private readonly IPaymentGateWay _paymentGateway;
        public OrderService(IUnitOfWork unitOfWork,ICartRepository cartRepo,IMapper mapper,IOrderConfirmationService confirmationService,IPaymentGateWay paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _cartRepo = cartRepo;
            _mapper = mapper;
            _confirmationService = confirmationService;
            _paymentGateway = paymentGateway;
        }

        public async Task<OrderDetailsDto> CreateOrderFromCartAsync(int userId, CreateOrderFromCartDto dto)
        {
            var cart = await _cartRepo.GetByIdAsync(dto.CartId);
            if (cart is null || !cart.CartItems.Any())
                throw new NotFoundException("Cart is empty or not found");

                var lines = new List<LineOrder>();
            foreach (var item in cart.CartItems)
            {
                
                lines.Add(new LineOrder(item.Id, item.Quantity));
            }
            var address = _mapper.Map<Address>(dto.ShippingAddress);
           var order = Order.CreatePending(userId,OrderSource.Cart,address,dto.PhoneNumber);
            await AddItemsToOrderAsync(order, lines);
            await _unitOfWork.GetGeneric<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            await _cartRepo.DeleteAsync(cart.Id);

            return _mapper.Map<OrderDetailsDto>(order);

        }

        public async Task<OrderDetailsDto> CreateOrderFromPrescriptionAsync(int userId, int prescriptionId)
        {
            var spec = new PrescriptionOrderSpecification(prescriptionId);
            var prescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec);
            if (prescriptionOrder is null)
                throw new NotFoundException($"There's Not found Exception with Id {prescriptionId}");
            if (prescriptionOrder.Status != PrescriptionStatus.ApprovedByCustomer)
                throw new BadRequestException("The prescription must be approved by the customer first");
            if (prescriptionOrder.OrderId != null)
                throw new BadRequestException("This prescription was already converted to an order");

            var lines = prescriptionOrder.Items
                .Select(i => new LineOrder(i.ProductId, i.Quantity)).ToList();

            var order = Order.CreatePending(userId, OrderSource.Prescription,
                prescriptionOrder.DeliveryAddress, prescriptionOrder.PhoneNumber);

            await AddItemsToOrderAsync(order, lines);

            if (order.Total != prescriptionOrder.TotalPrice)
                throw new BadRequestException("Prices have changed since you approved this prescription.");

            prescriptionOrder.Order = order;
            await _unitOfWork.GetGeneric<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDetailsDto>(order);


        }

        //customer 
        public async Task<PagedResult<OrderListItemDto>> GetMyOrdersAsync(OrderSpecParams spec,int userId)
        {
            
            var specification = new MyOrdersSpecification(spec,userId);
            var result = await _unitOfWork.GetGeneric<Order>().GetPagedAsync(specification);
            if (result is null)
                throw new NotFoundException($"tehre isn't user with Id {userId} ");

            var items = _mapper.Map<IEnumerable<OrderListItemDto>>(result.Items);

            return new PagedResult<OrderListItemDto>(items, result.PageNumber, result.PageSize, result.TotalCount);
            

        }
        
        public async Task<OrderDetailsDto> GetMyOrderWithIdAsync(int id, int userId)
        {
            var specification = new MyOrderDetailsSpecification(id, userId);
            var result = await _unitOfWork.GetGeneric<Order>().GetByIdAsync(specification);
            if (result is null)
                throw new NotFoundException($"there isn't order with Id {id}");

            return _mapper.Map<OrderDetailsDto>(result);
        }

        public async Task<PayOrderResultDto> PayMyOrderAsync(int id, int userId,PaymentMethod dto )
        {
            var spec = new MyOrderDetailsSpecification(id, userId);
            var order = await _unitOfWork.GetGeneric<Order>().GetByIdAsync(spec);
            if (order is null)
                throw new NotFoundException($"Order with Id {id} is not found");
            switch (dto)
            {
                case PaymentMethod.CashOnDelivery:
                    await _confirmationService.ConfirmAsync(order, PaymentMethod.CashOnDelivery);
                    await _unitOfWork.SaveChangesAsync();
                    return new PayOrderResultDto { Order = _mapper.Map<OrderDetailsDto>(order) };


                case PaymentMethod.Card:
                    return await  StartCardPaymentAsync(order);

                 default: throw new BadRequestException("Invalid handeled Method ");
            }
            
            
        }

        private async Task<PayOrderResultDto> StartCardPaymentAsync(Order order)
        {
            if (!order.CanBePaid)
                throw new BadRequestException("Order can only pay while pending or unpaid");
            await _confirmationService.EnsureStockAvailableAsync(order);

            var payment = Payment.Create(order.Id, order.Total);
            var result = await _paymentGateway.CreatePaymentAsync(new PaymentInitRequest(payment.MerchantRefNumber,
                payment.Amount,
                order.UserId,
                order.User.FirstName,
                order.User.LastName,
                order.User.Email!, 
                order.PhoneNumber,
                order.ShippingAddress.street
                ,order.ShippingAddress.City,
                order.OrderItems
                .Select(i => new PaymentInitItem(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice)).ToList()));

            payment.SetProvideReference(result.ProviderReference);
            await _unitOfWork.GetGeneric<Payment>().AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return new PayOrderResultDto
            {
                Order = _mapper.Map<OrderDetailsDto>(order),
                PaymentUrl = result.PaymentUrl,
                MerchantRefNumber = payment.MerchantRefNumber

            };
        }

        //admin

        public async Task<PagedResult<OrderListItemDto>> GetAllAsync(OrderSpecParams spec)
        {
            var specification = new OrderSpecification(spec);
            var result = await _unitOfWork.GetGeneric<Order>().GetPagedAsync(specification);

            var items = _mapper.Map<IEnumerable<OrderListItemDto>>(result.Items);
            return new PagedResult<OrderListItemDto>(items, result.PageNumber, result.PageSize, result.TotalCount);
        }

        public async Task<OrderDetailsDto> GetByIdAsync(int id)
        {
            var specification = new OrderSpecification(id);
            var result = await _unitOfWork.GetGeneric<Order>().GetByIdAsync(specification);
            if (result is null)
                throw new NotFoundException($"there isn't order with Id {id}");

            return _mapper.Map<OrderDetailsDto>(result);
        }

        public async Task<OrderDetailsDto> CancelMyOrderAsync(int id, int userId)
        {
            var specification = new MyOrderDetailsSpecification(id, userId);
            var order = await _unitOfWork.GetGeneric<Order>().GetByIdAsync(specification);

            if (order is null)
                throw new NotFoundException($"There isn't order with id {id}");
            var shouldReleaseStock = order.IsStockCommitted;
            
            order.Cancel();
            if (shouldReleaseStock)
                await _confirmationService.ReleaseStockAsync(order);
            
            await _unitOfWork.SaveChangesAsync();
           return  _mapper.Map<OrderDetailsDto>(order);
        }

        private async Task AddItemsToOrderAsync(Order order,IEnumerable<LineOrder> lines)
        {
            if (lines is null || !lines.Any())
                throw new BadRequestException("the order must contain at least one item");
            if (lines.Any(q => q.quantity <= 0))
                throw new BadRequestException("Quantity must be greater than 0");

            var mergedLines = new Dictionary<int, int>();

            foreach (var line in lines)
            {
                if (mergedLines.ContainsKey(line.productId))
                    mergedLines[line.productId] += line.quantity;
                else
                    mergedLines[line.productId] = line.quantity;
            }

            var result = mergedLines
                .Select(x => new LineOrder(x.Key, x.Value))
                .ToList();

            var productsId = result.Select(x => x.productId).ToList();

            var products = (await _unitOfWork.GetGeneric<Product>().GetAllAsync(p => productsId.Contains(p.Id)))
                .ToDictionary(p => p.Id);

            foreach(var line in result)
            {
                if (!products.TryGetValue(line.productId, out var product))
                    throw new NotFoundException($"Product {line.productId} is not found");
                
                
                if (line.quantity > product.Stock)
                   throw  new BadRequestException(
                     $"Only {product.Stock} of {product.Name} are found");
                order.AddItem(line.productId, line.quantity, product.Price);
            }

        }

     
    }

    public record LineOrder(int productId,int quantity);
}
