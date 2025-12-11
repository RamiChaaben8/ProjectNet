using System.Net.Http.Headers;

namespace FrontEnd.Services
{
    public class AuthorizingHttpMessageHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthorizingHttpMessageHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _authService.GetToken();
            
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
