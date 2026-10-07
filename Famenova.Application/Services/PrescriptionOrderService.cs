using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;

namespace Famenova.Application.Services
{
    public class PrescriptionOrderService : IPrescriptionOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrescriptionOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;   
        }
        //customer
         public async Task<PrescriptionOrderResponseDto>  CreateAsync(CreatePrescriptionOrderDto dto, int customerId)
        {
            var customer = await  _unitOfWork.GetGeneric<ApplicationUser>().GetByIdAsync(customerId);
            if (customer is null)
                throw new NotFoundException($"Customer with Id {customerId} is not found");

           var order= _mapper.Map<PrescriptionOrder>(dto);
            order.CustomerId = customerId;
            await _unitOfWork.GetGeneric<PrescriptionOrder>().AddAsync(order);
            await  _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PrescriptionOrder, PrescriptionOrderResponseDto>(order);



        }

       public  async Task<PrescriptionOrderResponseDto> RejectByCustomerAsync(int id, int customerId) {
            var spec = new MyPrescriptionDetailsSpecification(id,customerId);
            var prescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec);
            if (prescriptionOrder is null)
                throw new NotFoundException($"There isn't Prescription Order with Id {id}");
            prescriptionOrder.RejectByCustomer();
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(prescriptionOrder);


        }
        public async Task<OrderResponseDto> ApproveByCustomerAsync(int id, int customerId) { 
            var spec = new MyPrescriptionDetailsSpecification(id, customerId);
            var PrescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec);
            if (PrescriptionOrder is null)
                throw new NotFoundException($"there isn't prescriptionOrder with Id {id}");
            PrescriptionOrder.ApproveByCustomer();

            var order = Order.CreatePending(PrescriptionOrder.CustomerId,
                PrescriptionOrder.DeliveryAddress,
                PrescriptionOrder.PhoneNumber);

            foreach (var item in PrescriptionOrder.Items)
                order.AddItem(item.ProductId, item.Quantity, item.Price);

            PrescriptionOrder.Order = order;
            await _unitOfWork.GetGeneric<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<OrderResponseDto>(order);
        } 



        public async Task<PagedResult<PrescriptionOrderListItemDto>> GetMyPrescriptionOrderAsync(PrescriptionOrderSpecParams specParams, int customerId)
        {
            var spec = new MyPrescriptionsSpecification(specParams, customerId);
            var result = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetPagedAsync(spec);
            if (result is null)
                throw new NotFoundException($"tehre isn't customer with Id {customerId} ");
            var items = _mapper.Map<IEnumerable<PrescriptionOrderListItemDto>>(result.Items);

            return new PagedResult<PrescriptionOrderListItemDto>(items, result.PageNumber, result.PageSize, result.TotalCount);

        }
        public async Task<PrescriptionOrderDetailsDto> GetMyPrescriptionOrderAsync(int id, int customerId) {
            var spec = new MyPrescriptionDetailsSpecification(id, customerId);
            var result = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec) 
                ?? throw new NotFoundException($"There isn't Not found Prescription with Id {id}"); ;

            return _mapper.Map<PrescriptionOrderDetailsDto>(result);
        }

        //admin 
       public async Task<PrescriptionOrderResponseDto> RequestClarificationAsync(int id,string message) {

            var result = await GetForPharmacyAsync(id);
            result.RequestClarification(message);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);

        }
      public async Task<PrescriptionOrderResponseDto> ResumeReviewAsync(int id) {
            var result = await GetForPharmacyAsync(id);
            result.ResumeReview();
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);
        }
      public async Task<PrescriptionOrderResponseDto> RejectByPharmacyAsync(int id, RejectPrescriptionOrderDto rejectReason) {
            var result = await GetForPharmacyAsync(id);
            result.RejectByPharmacy(rejectReason.RejectReason);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);

        }
      public async Task<PrescriptionOrderResponseDto> AddItemAsync(int id, CreatePrescriptionOrderItemDto dto) {
            var result = await GetForPharmacyAsync(id);

            var product = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(dto.ProductId);
            if (product is null)
                throw new NotFoundException($"there is not found product with id {dto.ProductId}");
            result.AddItem(product, dto.Quantity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);
        }
      public async Task<PrescriptionOrderResponseDto> RemoveItemAsync(int id, int itemId) {
            var result = await GetForPharmacyAsync(id);

            result.RemoveItem(itemId);
           await  _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);

        }
      public async Task<PrescriptionOrderResponseDto> SendForCustomerApprovaleAsync(int Id) {
            var result = await GetForPharmacyAsync(Id);
            result.SendForCustomerApproval();
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);
        }
     
        public async Task<PagedResult<PrescriptionOrderListItemDto>> GetAllAsync(PrescriptionOrderSpecParams specParams) {
            var spec = new PrescriptionOrderSpecification(specParams);
            var result = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetPagedAsync(spec) ??
                throw new NotFoundException("There isn't Prescription");
            var items = _mapper.Map<IEnumerable<PrescriptionOrderListItemDto>>(result.Items);

            return new PagedResult<PrescriptionOrderListItemDto>(items, result.PageNumber, result.PageSize, result.TotalCount);
        
        }
      public async Task<PrescriptionOrderDetailsDto> GetByIdAsync(int id) {
         var result= await  GetForPharmacyAsync(id);
           return _mapper.Map<PrescriptionOrderDetailsDto>(result);
        
        }


        private async Task<PrescriptionOrder> GetForPharmacyAsync(int id)
        {
            var spec = new PrescriptionOrderSpecification(id);

            return await _unitOfWork
                       .GetGeneric<PrescriptionOrder>()
                       .GetByIdAsync(spec)
                   ?? throw new NotFoundException($"Prescription order with Id {id} is not found");
        }

        public async Task<PrescriptionOrderResponseDto> ProvideClarificationAsync(int id, int customerId, ProvideClarificationDto dto)
        {
            var spec = new MyPrescriptionDetailsSpecification(id, customerId);

            var prescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec)
                ?? throw new NotFoundException($"Prescription With Id {id} is not found");

            prescriptionOrder.ProiveClarfication(dto.Response, dto.ImageUrl);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(prescriptionOrder);
        }
    }
}
