using Singer.Core.Helper.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Singer.Core.Helper
{
    public enum GrantType
    {
        [Description("client_credentials")]
        ClientCredentials,
        [Description("password")]
        Password,
        [Description("authorization_code")]
        AuthorizationCode,
        [Description("refresh_token")]
        RefreshToken
    }

    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    public class OAuthHelper
    {
        private string _clientId;
        private string _clientSecret;
        private readonly string _host;
        private const string TokenCookie = "__huiwu_token";
        public string TokenPath { get; set; }

        public string AuthorizePath { get; set; }

        public OAuthHelper(string host, string clientId = null, string clientSecret = null)
        {
            TokenPath = "/token";
            AuthorizePath = "/authorize";
            _host = host;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public DResult<TokenResult> AccessToken(GrantType grantType, string refreshToken = null,
            string username = null, string password = null, string authorizationCode = null, string redirectUri = null)
        {
            var dict = new Dictionary<string, string>();
            switch (grantType)
            {
                case GrantType.Password:
                    dict.Add("grant_type", "password");
                    dict.Add("username", username);
                    dict.Add("password", password);
                    break;
                case GrantType.ClientCredentials:
                    break;
                case GrantType.AuthorizationCode:
                    dict.Add("grant_type", "authorization_code");
                    dict.Add("code", authorizationCode);
                    dict.Add("redirect_uri", redirectUri);
                    break;
                case GrantType.RefreshToken:
                    dict.Add("grant_type", "refresh_token");
                    dict.Add("refresh_token", refreshToken);
                    break;
            }
            var uri = new Uri(new Uri(_host), TokenPath);
            using (var http = new HttpHelper(uri.AbsoluteUri, HttpMethod.Post, Encoding.UTF8,
                string.Join("&", dict.Select(t => $"{t.Key}={t.Value}"))))
            {
                http.AddHeader("authorization",
                    "basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientId + ":" + _clientSecret)));
                http.AddHeader("cache-control", "no-cache");
                var html = http.Request();
                var data = JsonHelper.Json<TokenResult>(html);
                if (!string.IsNullOrWhiteSpace(data?.access_token))
                    return new DResult<TokenResult>(data);
                var result = JsonHelper.Json<DResult>(html);
                return new DResult<TokenResult>(result.Message, result.Code);
            }
        }
    }
}
