using BlazorServerCustomAuthTest.Models;

namespace BlazorServerCustomAuthTest.Auth
{
    public interface ILoginService
    {
        Task Login(UserProfile user);
        Task Logout();
    }
}
