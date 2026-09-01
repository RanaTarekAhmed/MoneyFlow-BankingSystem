using MoneyFlow.Business.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyFlow.Business.ViewModels.Customer
{
    public class CustomerIndexVM
    {
        public PagedResult<CustomerListVM> Customers { get; set; } = new();

        public string? Search { get; set; }
    }
}
