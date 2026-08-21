using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.ViewModels.Authentication;


namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterVM model);
        Task<SignInResult> LoginAsync(LoginVM model);
        Task LogoutAsync();
    }
}
