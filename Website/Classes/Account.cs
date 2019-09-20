using System;
using System.ComponentModel.DataAnnotations;
using Website.Models;

namespace Website.Classes
{
    public class Account
    {
        [Name]
        public string FirstName { get; set; }
        [Name]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Password]
        public string Password { get; set; }



        // ..................................................................................Create Customer.....................................................................
        public Customer CreateCustomer()
        {
            return new Customer
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                UserName = Guid.NewGuid().ToString("N"),
                Email = Email,
                FirstName = FirstName,
                LastName = LastName
            };
        }
    }
}
