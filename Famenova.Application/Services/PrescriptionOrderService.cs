using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Enums;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Interfaces;
using Famenova.Application.Services.Files;
using Famenova.Application.Specifications;
using Famenova.Shared;
using Famenova.Shared.Dtos.Order;
using Famenova.Shared.Dtos.PrescriptionOrder;
using Microsoft.AspNetCore.Authorization;

namespace Famenova.Application.Services
{
    public class PrescriptionOrderService : IPrescriptionOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public PrescriptionOrderService(IUnitOfWork unitOfWork, IMapper mapper,IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }
        //customer




        public async Task<PrescriptionOrderResponseDto> CreateAsync(CreatePrescriptionOrderDto dto, int customerId)
        {
            _ = await _unitOfWork.GetGeneric<ApplicationUser>().GetByIdAsync(customerId)
                ?? throw new NotFoundException($"Customer with Id {customerId} is not found");

            var notes = string.IsNullOrWhiteSpace(dto.CustomerNotes) ? null : dto.CustomerNotes.Trim();
            if (notes?.Length > 1000)
                throw new BadRequestException("Notes must be 1000 characters or fewer.");

            var order = _mapper.Map<PrescriptionOrder>(dto);
            order.CustomerId = customerId;
            order.CustomerNotes = notes;

            var image = await _fileService.SaveImageAsync(dto.PrescriptionImageUrl, "Prescriptions", allowPdf: true);
            order.PrescriptionImageUrl = image;

            try
            {
                await _unitOfWork.GetGeneric<PrescriptionOrder>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _fileService.DeleteImage(image);
                throw;
            }

            return _mapper.Map<PrescriptionOrderResponseDto>(order);
        }


        public async Task<PrescriptionOrderResponseDto> RejectByCustomerAsync(int id, int customerId) {
            var spec = new MyPrescriptionDetailsSpecification(id,customerId);
            var prescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec);
            if (prescriptionOrder is null)
                throw new NotFoundException($"There isn't Prescription Order with Id {id}");
            prescriptionOrder.RejectByCustomer();
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(prescriptionOrder);


        }
        public async Task<OrderDetailsDto> ApproveByCustomerAsync(int id, int customerId) { 
            var spec = new MyPrescriptionDetailsSpecification(id, customerId);
            var PrescriptionOrder = await _unitOfWork.GetGeneric<PrescriptionOrder>().GetByIdAsync(spec);
            if (PrescriptionOrder is null)
                throw new NotFoundException($"there isn't prescriptionOrder with Id {id}");
            PrescriptionOrder.ApproveByCustomer();

            var order = Order.CreatePending(PrescriptionOrder.CustomerId,OrderSource.Prescription,PrescriptionOrder.DeliveryAddress,PrescriptionOrder.PhoneNumber);

            foreach (var item in PrescriptionOrder.Items)
                order.AddItem(item.ProductId, item.Quantity, item.Price);

            PrescriptionOrder.Order = order;
            await _unitOfWork.GetGeneric<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<OrderDetailsDto>(order);
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
            var prescription = await _unitOfWork.GetGeneric<PrescriptionOrder>()
                .GetByIdAsync(new MyPrescriptionDetailsSpecification(id, customerId))
                ?? throw new NotFoundException($"Prescription With Id {id} is not found");

            if (string.IsNullOrWhiteSpace(dto.Response) && dto.ImageUrl is null)
                throw new BadRequestException("Write a reply or attach a photo.");

            string? img = null;
            try
            {
                if (dto.ImageUrl is not null)
                    img = await _fileService.SaveImageAsync(dto.ImageUrl, "Prescriptions", allowPdf: true);

                prescription.ProiveClarfication(dto.Response?.Trim(), img);   // throws if the status is wrong
                await _unitOfWork.GetGeneric<PrescriptionMessage>().AddAsync(PrescriptionMessage.Create(
                    prescription.Id, MessageSender.Customer, customerId,
                    PrescriptionMessageKind.ClarificationReply, dto.Response, img));
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (img is not null) _fileService.DeleteImage(img);            // no orphan file
                throw;
            }

            return _mapper.Map<PrescriptionOrderResponseDto>(prescription);
        }
        public async Task<PrescriptionOrderResponseDto> RequestClarificationAsync(int id, string message, int? pharmacistId = null)
        {
            var result = await GetForPharmacyAsync(id);
            result.RequestClarification(message);
            // the question stays in the conversation, even after a later question replaces it on the order
            await _unitOfWork.GetGeneric<PrescriptionMessage>().AddAsync(PrescriptionMessage.Create(
                result.Id, MessageSender.Pharmacist, pharmacistId,
                PrescriptionMessageKind.ClarificationRequest, message, null));
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PrescriptionOrderResponseDto>(result);

        }

        // ================= Conversation =================

        public async Task<IReadOnlyList<PrescriptionMessageDto>> GetMessagesAsync(int id, int userId, bool asPharmacist)
        {
            await GetForConversationAsync(id, userId, asPharmacist);
            var messages = await _unitOfWork.GetGeneric<PrescriptionMessage>()
                .ListAsync(new PrescriptionMessagesSpecification(id));
            return _mapper.Map<List<PrescriptionMessageDto>>(messages);
        }

        /// <summary>
        /// A message from either side. While the prescription waits for the customer's answer
        /// (NeedsClarification), a customer message IS the answer: it goes through the same domain rule
        /// as provideClarification, so the pharmacist sees "customer replied".
        /// </summary>
        public async Task<PrescriptionMessageDto> SendMessageAsync(int id, int userId, bool asPharmacist, SendPrescriptionMessageDto dto)
        {
            var prescription = await GetForConversationAsync(id, userId, asPharmacist);

            if (prescription.IsClosed)
                throw new BadRequestException("This prescription is closed: the conversation can't receive new messages.");
            if (string.IsNullOrWhiteSpace(dto.Body) && dto.Attachment is null)
                throw new BadRequestException("Write a message or attach a file.");
            if (dto.Body?.Trim().Length > PrescriptionMessage.MaxBodyLength)
                throw new BadRequestException($"A message can be at most {PrescriptionMessage.MaxBodyLength} characters.");

            var isReply = !asPharmacist && prescription.Status == PrescriptionStatus.NeedsClarification;

            string? file = null;
            try
            {
                if (dto.Attachment is not null)
                    file = await _fileService.SaveImageAsync(dto.Attachment, "PrescriptionMessages", allowPdf: true);

                if (isReply)
                    prescription.ProiveClarfication(dto.Body?.Trim(), file);

                var message = PrescriptionMessage.Create(
                    prescription.Id,
                    asPharmacist ? MessageSender.Pharmacist : MessageSender.Customer,
                    userId,
                    isReply ? PrescriptionMessageKind.ClarificationReply : PrescriptionMessageKind.Message,
                    dto.Body,
                    file);

                await _unitOfWork.GetGeneric<PrescriptionMessage>().AddAsync(message);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<PrescriptionMessageDto>(message);
            }
            catch
            {
                if (file is not null) _fileService.DeleteImage(file);   // no orphan file
                throw;
            }
        }

        /// <summary>Marks the OTHER side's messages of this prescription as read; returns how many</summary>
        public async Task<int> MarkMessagesReadAsync(int id, int userId, bool asPharmacist)
        {
            await GetForConversationAsync(id, userId, asPharmacist);
            var from = asPharmacist ? MessageSender.Customer : MessageSender.Pharmacist;

            var unread = (await _unitOfWork.GetGeneric<PrescriptionMessage>()
                .GetAllAsync(m => m.PrescriptionOrderId == id && m.Sender == from && m.ReadAtUtc == null))
                .ToList();
            if (unread.Count == 0) return 0;

            foreach (var m in unread) m.MarkRead();
            await _unitOfWork.SaveChangesAsync();
            return unread.Count;
        }

        public async Task<UnreadMessagesSummaryDto> GetUnreadAsync(int userId, bool asPharmacist)
        {
            var repo = _unitOfWork.GetGeneric<PrescriptionMessage>();
            var unread = (asPharmacist
                    ? await repo.GetAllAsync(m => m.Sender == MessageSender.Customer && m.ReadAtUtc == null)
                    : await repo.GetAllAsync(m => m.Sender == MessageSender.Pharmacist && m.ReadAtUtc == null
                                                  && m.PrescriptionOrder.CustomerId == userId))
                .ToList();

            var items = unread
                .GroupBy(m => m.PrescriptionOrderId)
                .Select(g =>
                {
                    var last = g.OrderByDescending(m => m.SentAtUtc).First();
                    return new UnreadMessagesItemDto
                    {
                        PrescriptionOrderId = g.Key,
                        Count = g.Count(),
                        LastSentAtUtc = last.SentAtUtc,
                        IsClarification = last.Kind != PrescriptionMessageKind.Message
                    };
                })
                .OrderByDescending(i => i.LastSentAtUtc)
                .ToList();

            return new UnreadMessagesSummaryDto { Total = unread.Count, Items = items };
        }

        /// <summary>Staff see every prescription; a customer only their own (404 otherwise)</summary>
        private async Task<PrescriptionOrder> GetForConversationAsync(int id, int userId, bool asPharmacist)
        {
            if (asPharmacist) return await GetForPharmacyAsync(id);

            return await _unitOfWork.GetGeneric<PrescriptionOrder>()
                       .GetByIdAsync(new MyPrescriptionDetailsSpecification(id, userId))
                   ?? throw new NotFoundException($"Prescription With Id {id} is not found");
        }
    }
}
