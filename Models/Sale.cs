using System;
using System.Collections.Generic;

namespace jwt_security_token_handler_asymmetric.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public virtual Client Client { get; set; }
        public Guid ClientId { get; set; }
        public virtual IEnumerable<SaleItem> Items { get; set; }
    }
}