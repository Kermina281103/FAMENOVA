using famenova.Domain.Common;
using famenova.Domain.Enums;
using famenova.Domain.Exceptions;
using Famenova.Shared.Dtos.PrescriptionOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    //الروشته 
    public class PrescriptionOrder : BaseAuditableEntity<int>
    {
        public string PrescriptionImageUrl { get; set; } = default!;
        public Address DeliveryAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.PendingReview;

        public string? RejectReason { get; set; }
        public decimal? TotalPrice { get; set; }

        public int CustomerId { get; set; }
        public ApplicationUser Customer { get; set; } = default!;
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public string? ClarificationRequest { get; set; } // admin what need
        public string? ClarificationResponse { get; set; } ///customer response
       
        public string? ClarificationImageUrl { get; set; } // adding image by customer
        public DateTime? ClarificationRespondedAt { get; set; } 

        public ICollection<PrescriptionOrderItem> Items { get; set; } = new HashSet<PrescriptionOrderItem>();





        public void RequestClarification(string message)
        {
            if (Status != PrescriptionStatus.PendingReview)
                throw new DomainException("Clarification can only be requested for orders under review.");

            if (string.IsNullOrEmpty(message))
                throw new DomainException("Clarification Message is Required");
            ClarificationRequest = message;
            ClarificationResponse = null;
            ClarificationRespondedAt = null;
            ClarificationImageUrl = null;

            Status = PrescriptionStatus.NeedsClarification;
        }


        public void ProiveClarfication(string? response, string? imageUrl)
        {
            if (Status != PrescriptionStatus.NeedsClarification)
                throw new DomainException("Clarification can only be provided with need Clarfication");

            if (string.IsNullOrEmpty(response) && string.IsNullOrEmpty(imageUrl))
                throw new DomainException("You must provide a response or imageUrl");
            ClarificationResponse = response;
            ClarificationImageUrl = imageUrl;
            ClarificationRespondedAt = DateTime.UtcNow;
        }

        public void RejectByPharmacy(string rejectReason)
        {
            if (Status != PrescriptionStatus.PendingReview&& Status != PrescriptionStatus.NeedsClarification)
                throw new DomainException("Rejected By Pharmacy can only be Requested for order under Review or need clarification");
            
            if (rejectReason is null)
                throw new DomainException("Reject reason is required.");
            RejectReason = rejectReason;
            Status = PrescriptionStatus.RejectedByPharmacy;
        }

        public void ResumeReview()
        {
            if (Status != PrescriptionStatus.NeedsClarification)
                throw new DomainException("Review can only be resumed for orders that need clarification.");
            Status = PrescriptionStatus.PendingReview;
        }
        public void SendForCustomerApproval()
        {
            if (Status != PrescriptionStatus.PendingReview)
                throw new DomainException("Customer Approval can only be Requested for order under Review");
            if (!Items.Any())
                throw new DomainException("You must add the items");

            Status = PrescriptionStatus.AwaitingCustomerApproval;
        }

       

        //items
        public void AddItem( Product product,int quantity)
        {
            EnsureItemsCanBeModified();
            if (product is null)
                throw new DomainException("Product is Required");
            if (quantity <= 0)
                throw new DomainException("Quantity must be greather than zero");

           
            if (Items.Any(i => i.ProductId == product.Id))
                throw new DomainException("This product is already in the list");
            if(quantity>product.Stock)
                throw new DomainException( $"Only {product.Stock} unit(s) of '{product.Name}' are currently in stock.");

            Items.Add(new PrescriptionOrderItem
            {
                ProductId = product.Id,
                Product =product,
                Quantity = quantity,
                Price = product.Price
            });

            RecalculateTotal();
        }

        public void RemoveItem(int itemId)
        {
            EnsureItemsCanBeModified();
            var item = Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException("Item is not found in this prscription Order");
            Items.Remove(item);
            RecalculateTotal();
        }
        private void RecalculateTotal()
        {
            TotalPrice = Items.Sum(i => i.Price * i.Quantity);
        }

        private void EnsureItemsCanBeModified()
        {
            if (Status != PrescriptionStatus.PendingReview)
                throw new DomainException("Items can only be modified when Status is PendingReview");
        }

       ///Customer Decision 

        public void RejectByCustomer()
        {
            if (Status != PrescriptionStatus.AwaitingCustomerApproval)
                throw new DomainException("Order can only be rejected when status is Awaiting ");

            Status = PrescriptionStatus.RejectedByCustomer;
        }
        public void ApproveByCustomer()
        {
            if (Status != PrescriptionStatus.AwaitingCustomerApproval)
                throw new DomainException("Order can only be approved while status is Awaiting customer approved");

            if (!Items.Any())
                throw new DomainException("Cannot approove a prescription without items");
            Status = PrescriptionStatus.ApprovedByCustomer;
        }

    }
}