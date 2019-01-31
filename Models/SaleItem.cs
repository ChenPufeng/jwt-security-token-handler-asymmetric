using System;

namespace jwt_security_token_handler_asymmetric.Models
{
    public class SaleItem
    {
        public Guid Id { get; set; }
        public virtual  Sale Sale { get; set; }
        public Guid SaleId { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}