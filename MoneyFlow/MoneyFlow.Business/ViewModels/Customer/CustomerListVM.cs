using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyFlow.Business.ViewModels.Customer
{
    public class CustomerListVM
    {
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string NationalId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public int AccountsCount { get; set; }
    }
}
