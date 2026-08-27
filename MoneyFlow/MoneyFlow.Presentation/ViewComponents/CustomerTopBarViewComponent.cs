using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using System.Security.Claims;


namespace MoneyFlow.Presentation.ViewComponents
{
    public class CustomerTopBarViewComponent : ViewComponent
    {
        private readonly ICustomerService _customerService;

        public CustomerTopBarViewComponent(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _customerService.GetCustomerTopBarDataAsync(userId);

            return View(result);
        }
    }
}
