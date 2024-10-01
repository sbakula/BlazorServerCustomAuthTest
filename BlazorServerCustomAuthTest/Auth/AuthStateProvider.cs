using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using BlazorServerCustomAuthTest.Models;

namespace BlazorServerCustomAuthTest.Auth

{
    public class AuthStateProvider : AuthenticationStateProvider, ILoginService
    {
        private ProtectedSessionStorage ProtectedSessionStore;

        static AuthenticationState Anonymous =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public AuthStateProvider(ProtectedSessionStorage ProtectedSessionStore)
        {
            this.ProtectedSessionStore = ProtectedSessionStore;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            UserProfile? user;
            var result = await ProtectedSessionStore.GetAsync<UserProfile>("UserProfile");

            if (result.Success)
            {
                user = result.Value;
                return await Task.FromResult(BuildAuthenticationState(user));
            }
            else
            {
                return Anonymous;
            }
        }

        public async Task Login(UserProfile user)
        {
            await ProtectedSessionStore.SetAsync("UserProfile", user);
            var authState = BuildAuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await ProtectedSessionStore.DeleteAsync("UserProfile");
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        static AuthenticationState BuildAuthenticationState(UserProfile? userProfile)
        {
            if (userProfile is null)
            {
                return Anonymous;
            }
            else
            {
                var claims = new List<Claim> { };
                claims.Add(new Claim(ClaimTypes.Name, userProfile.UserName));
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
            }
        }

    }
}
