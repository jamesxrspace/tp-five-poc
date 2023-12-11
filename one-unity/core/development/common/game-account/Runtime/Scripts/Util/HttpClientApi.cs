namespace TPFive.Game.Account
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;

    public class HttpClientApi
    {
        protected static readonly HttpClient Client = new HttpClient();

        private static readonly string TAG = "[XRAccount][HttpClientApi]";
        private static readonly string TagOfSignInName = "email";

        /*
         * openid: Required (Indicates that the token is an OIDC token)
         * offline_access: Requests a refresh token
         * profile: Requests access to the user's profile, we need nickname
         * username: Requests access to the user's username
         * email: Requests access to the user's email and email_verified
         */
        private static readonly string OidcScope = "openid offline_access profile username email";

        public HttpClientApi(string domain, string clientId, string accountServer)
        {
            Domain = domain;
            ClientID = clientId;
            AccountServer = accountServer;
        }

        public string ClientID { get; private set; }

        public string Domain { get; private set; }

        public string AccountServer { get; private set; }

        public Task<HttpResponseMessage> HttpSignIn(string name, string password)
        {
            // URL
            string urlString = GetUrlString(Domain, AccountConst.AuthingTokenEndpoint);
            Debug.Log($"{TAG} urlString: {urlString}");

            // Body
            var formBody = new Dictionary<string, string>
            {
                { "client_id", ClientID },
                { "grant_type", "password" },
                { "password", password },
                { TagOfSignInName, name },
                { "scope", OidcScope },
            };
            var content = new FormUrlEncodedContent(formBody);

            // Header
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var request = new HttpRequestMessage(HttpMethod.Post, urlString)
            {
                Content = content,
            };
            return Client.SendAsync(request);
        }

        public Task<HttpResponseMessage> HttpRenewAuth(string refreshToken)
        {
            string urlString = GetUrlString(Domain, AccountConst.AuthingTokenEndpoint);
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "client_id", ClientID },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
            };
            var content = new FormUrlEncodedContent(body);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var request = new HttpRequestMessage(HttpMethod.Post, urlString) { Content = content };
            return Client.SendAsync(request);
        }

        public Task<HttpResponseMessage> StartDeviceAuth()
        {
            string urlString = GetUrlString(AccountServer, AccountConst.DeviceAuthEndpoint);
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "client_id", ClientID },
                { "scene", "APP_AUTH" },
            };
            var content = new FormUrlEncodedContent(body);
            var request = new HttpRequestMessage(HttpMethod.Post, urlString) { Content = content };
            return Client.SendAsync(request);
        }

        public Task<HttpResponseMessage> PollingDeviceCodeStatus(string userCode)
        {
            string urlString = GetUrlString(AccountServer, AccountConst.DeviceAuthStatusEndpoint, $"client_id={ClientID}&user_code={userCode}");
            var request = new HttpRequestMessage(HttpMethod.Get, urlString);
            return Client.SendAsync(request);
        }

        public Task<HttpResponseMessage> AuthingTokenToCredential(string authingToken)
        {
            string urlString = GetUrlString(Domain, AccountConst.DeviceAuthingIdTokenToAccessTokenEndpoint);
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "client_id", ClientID },
                { "grant_type", AccountConst.IdTokenToCredentialsGrantType },
                { "redirect_uri", AccountConst.IdTokenToCredentialsRedirectUri },
                { "token", authingToken },
                { "scope", "openid profile email phone" },
            };

            var content = new FormUrlEncodedContent(body);
            var request = new HttpRequestMessage(HttpMethod.Post, urlString) { Content = content };
            return Client.SendAsync(request);
        }

        public Task<HttpResponseMessage> CreateGuestAccount(string nickname)
        {
            string urlString = GetUrlString(AccountServer, AccountConst.GuestCreateEndpoint);
            string jsonString = $"{{\"nickname\":\"{nickname}\"}}";
            Debug.Log($"{TAG} Request body = {jsonString}");
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, urlString) { Content = content };
            return Client.SendAsync(request);
        }

        private static string GetUrlString(string domain, string endpoints, string queryString = null)
        {
            Uri uri = new Uri(domain);
            return new UriBuilder
            {
                Scheme = uri.Scheme,
                Host = uri.Host,
                Port = uri.Port,
                Path = endpoints,
                Query = queryString,
            }.ToString();
        }
    }
}
