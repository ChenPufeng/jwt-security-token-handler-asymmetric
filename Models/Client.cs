using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace jwt_security_token_handler_asymmetric.Models
{
    public class Client
    {
        public Guid Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Filed {0} is required")]
        public string Name { get; set; }
        
        [DisplayName("Bird Date")]
        public DateTime Bird { get; set; }
        
        [DisplayName("Client is VIP")]
        public bool IsVip { get; set; }
        
        [DisplayName("Adress")]
        public string Adress { get; set; }
        
        [DisplayName("Phone number")]
        public string Phone { get; set; }
        
        [DisplayName("E-mail")]
        public string Email { get; set; }
        public virtual IEnumerable<Sale> Sales { get; set; }
    }
}