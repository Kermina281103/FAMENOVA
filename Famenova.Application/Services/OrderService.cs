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

namespace Famenova.Application.Services
{

    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork,ICartRepository cartRepo,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartRepo = cartRepo;
            _mapper = mapper;
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
            var result = await _unitOfWork.GetGeneric<Order>().GetByIdAsync(specification);

            if (result is null)
                throw new NotFoundException($"There isn't order with id {id}");
            var result2 = result;
            result.Cancel();
            _unitOfWork.GetGeneric<Order>().Remove(result);
            await _unitOfWork.SaveChangesAsync();
           return  _mapper.Map<OrderDetailsDto>(result2);
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
