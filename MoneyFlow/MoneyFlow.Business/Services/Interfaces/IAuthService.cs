using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.ViewModels.Authentication;
using MoneyFlow.Data.Entities;


namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);
        Task<SignInResult> LoginAsync(LoginVM model);
        Task LogoutAsync();
        AuthenticationProperties GetExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<ExternalAuthOutcome> HandleExternalLoginCallbackAsync();
        Task<ExternalRegisterVM?> GetExternalRegisterPrefillAsync();
        Task<(IdentityResult Result, ApplicationUser? User)> CompleteExternalRegisterAsync(ExternalRegisterVM model);
    }
}
