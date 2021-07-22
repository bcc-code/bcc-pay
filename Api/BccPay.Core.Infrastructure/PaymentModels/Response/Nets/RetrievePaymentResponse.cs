using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using System.Collections.Generic;

namespace BccPay.Core.Infrastructure.PaymentModels.Response.Nets
{
    public class RetrievePaymentResponse
    {
        public string PaymentId { get; set; }
        public Summary Summary { get; set; }
        public Consumer Consumer { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
        public OrderDetails MyProperty { get; set; }
        public Checkout Checkout { get; set; }
        public string Created { get; set; }
        public List<Refund> Refunds { get; set; }
        public List<Charge> Charges { get; set; }
        public string Terminated { get; set; }
        public Subscription Subscription { get; set; }
    }

    public class Summary
    {
        public int ReservedAmount { get; set; }
        public int ChargedAmount { get; set; }
        public int RefundedAmount { get; set; }
        public int CancelledAmount { get; set; }
    }

    public class Consumer
    {
        public Address ShippingAddress { get; set; }
        public Company Company { get; set; }
        public PrivatePerson PrivatePerson { get; set; }
        public Address BillingAddress { get; set; }
    }

    public class PrivatePerson
    {
        public string MerchantReference { get; set; }
        public string DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }

    public class Company
    {
        public string MerchantReference { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public ContactDetails ContactDetails { get; set; }
    }

    public class ContactDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }

    public class PhoneNumber
    {
        public string Prefix { get; set; }
        public string Number { get; set; }
    }


    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ReceiverLine { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class PaymentDetails
    {
        /// <summary>
        /// The type of payment. Possible values are: 'CARD', 'INVOICE', 'A2A', 'INSTALLMENT', 'WALLET', and 'PREPAID-INVOICE'.
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// The payment method, for example Visa or Mastercard.
        /// </summary>
        public string PaymentMethod { get; set; }
        public InvoiceDetails InvoiceDetails { get; set; }
        public CardDetails CardDetails { get; set; }
    }

    public class CardDetails
    {
        /// <summary>
        /// A masked version of the PAN (Primary Account Number). At maximum, only the first six and last four digits of the account number are displayed.
        /// </summary>
        public string MaskedPan { get; set; }
        public string ExpiryDate { get; set; }
    }

    public class InvoiceDetails
    {
        public string InvoiceNumber { get; set; }
        public string Orc { get; set; }
        public string PdfLink { get; set; }
        public string DueDate { get; set; }
    }

    public class OrderDetails
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Reference { get; set; }
    }

    public class Checkout
    {
        public string Url { get; set; }
        public string CancelUrl { get; set; }
    }


    public class Refund
    {
        public string RefundId { get; set; }
        public int Amount { get; set; }
        public string State { get; set; }
        public string LastUpdated { get; set; }
        public List<Item> OrderItems { get; set; }
    }

    public class Charge
    {
        public string ChargeId { get; set; }
        public int Amount { get; set; }
        public string Created { get; set; }
        public List<Item> OrderItems { get; set; }
    }


    public class Subscription
    {
        public string Id { get; set; }
    }
}
