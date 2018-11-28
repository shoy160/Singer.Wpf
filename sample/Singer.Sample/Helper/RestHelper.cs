using ESignature.Business;
using Singer.Core;
using Singer.Core.Helper;
using Singer.Core.Helper.Json;
using Singer.Core.Logging;
using Singer.Sample.AppService;
using Singer.Sample.Dtos;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace Singer.Sample.Helper
{
    public class RestHelper
    {
        private TokenResult _token;
        private readonly ILogger _logger;
        private RestHelper()
        {
            _logger = LogManager.Logger<RestHelper>();
            _token = new GlobalDataService().Query<TokenResult>(GlobalKeys.AccessToken);
        }

        public static RestHelper Instance => Singleton<RestHelper>.Instance ??
                                             (Singleton<RestHelper>.Instance = new RestHelper());

        /// <summary> 获取API接口返回的实体对象 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private T Request<T>(string api, object paras = null, HttpMethod method = HttpMethod.Get, string contentType = null)
            where T : DResult, new()
        {
            if (!Constants.IsConnected())
                return new T { Status = true, Message = "无网络连接" };
            var uri = new Uri(new Uri(Const.Host), api);
            string ps;
            if (method != HttpMethod.Post)
            {
                var dict = paras.ToDictionary();
                ps = string.Join("&",
                    dict.Select(t => $"{t.Key}={HttpUtility.UrlEncode(t.Value ?? string.Empty, Encoding.UTF8)}"));
            }
            else
            {
                ps = JsonHelper.ToJson(paras);
            }

            using (var http = new HttpHelper(uri.AbsoluteUri, method, Encoding.UTF8, ps))
            {
                http.SetUserAgent(Const.UserAgent);
                if (_token != null)
                    http.AddHeader("authorization", $"{_token.token_type} {_token.access_token}");
                if (!string.IsNullOrWhiteSpace(contentType))
                    http.SetContentType(contentType);
                var html = http.Request(true);
                try
                {
                    if (string.IsNullOrWhiteSpace(html))
                        return new T { Status = false, Message = "无数据" };
                    var result = JsonHelper.Json<T>(html);
                    if (result.Status)
                        return result;
                    _logger.Warn($"{api}:{ps},{html}");
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    return new T { Status = false, Message = "服务器数据异常" };
                }
            }
        }

        public DResult<T> GetResult<T>(string api, object paras = null, HttpMethod method = HttpMethod.Get, string contentType = null)
        {
            return Request<DResult<T>>(api, paras, method, contentType);
        }

        /// <summary> 获取API接口返回的实体对象 </summary>
        /// <param name="api"></param>
        /// <param name="paras"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public DResult GetResult(string api, object paras = null, HttpMethod method = HttpMethod.Get, string contentType = null)
        {
            return Request<DResult>(api, paras, method, contentType);
        }

        public ManifestDto GetVersion()
        {
            return new ManifestDto
            {
                Version = "1.0.1",
                DownloadUrl = "http://file.dayeasy.net/update/markingTool/MarkingTool_0.6.29.exe",
                Mandatory = false,
                Md5 = Guid.NewGuid(),
                UpgradeInstructions = "1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能；1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能。1：向上滑动的按钮，用RepeatButton实现功能；\r\n2：上部分滑块，功能同1，也是一个RepeatButton来实现的；\r\n3：中间可拖动滑块，用一个Thumb来实现；\r\n4：下部分滑块，和5功能一样，向下滑动，用一个RepeatButton来实现；\r\n5：向下滑动的按钮，用RepeatButton实现功能。",
                UpgradeTime = DateTime.Now
            };
            var result = GetResult<ManifestDto>(GlobalKeys.ApiManifest);
            return result.Data ?? new ManifestDto { Version = "0.0.0" };
        }
        /// <summary> 用户登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DResult Login(string account, string password)
        {
            //var service = GlobalDataService.Instance;
            //var client = service.Query<ClientSecretDto>(GlobalKeys.ClientSecret);
            //var helper = new OAuthHelper(Const.Host, client.Id, client.Secret);
            //var token = helper.AccessToken(GrantType.Password, username: account, password: password);
            //if (!token.IsSuccess)
            //    return token;
            //_token = token.Data;
            //new GlobalDataService().InsertOrUpdate(GlobalKeys.AccessToken, token.Data);
            //return token;
            return DResult.Success;
        }

        public void Logout()
        {
            Instance.GetResult(GlobalKeys.ApiLogout, method: HttpMethod.Post);
        }

        /// <summary> 获取用户信息 </summary>
        public UserInfoDto UserInfo()
        {
            return new UserInfoDto
            {
                Id = "123",
                Name = "shay",
                Permission = 1,
                LoginDate = DateTime.Now
            };
            //var result = GetResult<UserInfoDto>(GlobalKeys.ApiUserInfo);
            //if (!result.IsSuccess)
            //{
            //    return null;
            //}
            //GlobalDataService.Instance.InsertOrUpdate(GlobalKeys.LoginUser, result.Data);
            //return result.Data;
        }
    }
}
