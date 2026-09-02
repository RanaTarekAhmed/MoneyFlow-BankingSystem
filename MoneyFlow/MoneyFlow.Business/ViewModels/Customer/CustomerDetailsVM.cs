using MoneyFlow.Business.ViewModels.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyFlow.Business.ViewModels.Customer
{
    public class CustomerDetailsVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }

        public List<AccountSummaryVM> Accounts { get; set; } = new();
        public List<TransactionVM> Transactions { get; set; } = new();
    }
}
