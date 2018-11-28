using Singer.Core.Helper.Json;
using Singer.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Singer.Core.Helper
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }

    /// <summary> http请求类~ create by shy --2012-08-17 </summary>
    public class HttpHelper : IDisposable
    {
        private string _url;//
        private readonly HttpMethod _method;
        private readonly string _referer;
        private readonly string _paras;
        private readonly Encoding _encoding;//编码
        private static string _cookie;
        private string _contentType = "application/x-www-form-urlencoded";
        private Dictionary<string, Stream> _fileList;
        private readonly IDictionary<string, string> _headers;

        private MemoryStream _postStream;
        private WebProxy _proxy;

        private HttpWebRequest _req;
        private HttpWebResponse _rep;
        /// <summary> 空字符 </summary>
        private static readonly string Empty = string.Empty;

        private static readonly string Boundary = "-------------" + DateTime.Now.Ticks.ToString("x");
        private static readonly string NewLine = Environment.NewLine;

        private string _userAgent;
        private readonly ILogger _logger = LogManager.Logger<HttpHelper>();

        #region 构造函数

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        public HttpHelper(string url)
            : this(url, HttpMethod.Get, Encoding.Default, Empty, Empty, Empty)
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        public HttpHelper(string url, Encoding encoding)
            : this(url, HttpMethod.Get, encoding, Empty, Empty, Empty)
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="encoding"></param>
        /// <param name="paras"></param>
        public HttpHelper(string url, HttpMethod method, Encoding encoding, string paras)
            : this(url, method, encoding, Empty, Empty, paras)
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="method">请求方法</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="cookie">请求Cookie</param>
        /// <param name="referer">"base"为当前url域名</param>
        /// <param name="paras"></param>
        public HttpHelper(string url, HttpMethod method, Encoding encoding, string cookie, string referer, string paras)
        {
            _url = url;
            _method = method;
            if (!string.IsNullOrEmpty(cookie))
                _cookie = cookie;
            if (!string.IsNullOrEmpty(referer))
                _referer = referer;
            if (!string.IsNullOrEmpty(paras))
                _paras = paras;
            _encoding = encoding;
            _headers = new Dictionary<string, string>
            {
                {"Accept-language", "zh-cn,zh;q=0.5"},
                {"Accept-Charset", "utf-8;q=0.7,*;q=0.7"},
                {"Accept-Encoding", "gzip, deflate"},
                {"Keep-Alive", "350"},
                {"x-requested-with", "XMLHttpRequest"}
            };
        }

        #endregion

        /// <summary> 创建httpwebrequest 实例 </summary>
        private void CreateHttpRequest()
        {
            if (string.IsNullOrEmpty(_url))
                return;
            if (!_url.StartsWith("http://") && !_url.StartsWith("https://"))
                _url = "http://" + _url;
            if (_method != HttpMethod.Post && !string.IsNullOrWhiteSpace(_paras))
            {
                var t = _url.Contains("?") ? "&" : "?";
                _url = $"{_url}{t}{_paras}";
            }
            _req = (HttpWebRequest)WebRequest.Create(_url);

            _req.AllowAutoRedirect = true;

            _req.Method = _method.ToString().ToUpper();

            _req.Timeout = 20 * 60 * 1000;

            _req.ServicePoint.ConnectionLimit = 1024;

            if (_headers.Any())
            {
                foreach (var header in _headers)
                {
                    _req.Headers.Add(header.Key, header.Value);
                }
            }
            _req.UserAgent = _userAgent;
            //代理设置
            if (_proxy != null)
            {
                _req.Proxy = _proxy;
                //设置安全凭证
                _req.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            //添加Cookie
            if (!string.IsNullOrEmpty(_cookie))
                _req.Headers.Add("Cookie", _cookie);
            if (!string.IsNullOrEmpty(_referer))
            {
                if (_referer == "base")
                {
                    var baseUrl = new Regex("http(s)?://([^/]+?)/", RegexOptions.IgnoreCase).Match(_url).Groups[2]
                        .Value;
                    _req.Referer = baseUrl;
                }
                else
                {
                    _req.Referer = _referer;
                }
            }
            if (_method == HttpMethod.Put || _method == HttpMethod.Delete || _fileList != null && _fileList.Any())
            {
                _req.ContentLength = 0;
                _req.ContentType = $"multipart/form-data; boundary={Boundary}";
            }
            else
                _req.ContentType = _contentType;

            if (_method != HttpMethod.Post)
                return;
            WriteParams(_paras);
            //传文件
            if (_fileList != null && _fileList.Any())
            {
                _req.AllowWriteStreamBuffering = false;
                _req.KeepAlive = true;
                foreach (var file in _fileList)
                {
                    WriteFileStream(file.Key, file.Value);
                }
                var strBoundary = string.Format("{1}--{0}--{1}", Boundary, NewLine);
                WriteParams(strBoundary);
            }
            if (_postStream == null)
                return;
            _req.ContentLength = _postStream.Length;
            var postStream = _req.GetRequestStream();
            var buffer = new byte[checked((uint)Math.Min(4096, (int)_postStream.Length))];
            int bytesRead;
            _postStream.Seek(0, SeekOrigin.Begin);
            while ((bytesRead = _postStream.Read(buffer, 0, buffer.Length)) != 0)
                postStream.Write(buffer, 0, bytesRead);
            _postStream.Close();
            _postStream.Dispose();
        }

        private void WriteRequestStream(byte[] buffer, int count)
        {
            if (_postStream == null)
                _postStream = new MemoryStream();
            _postStream.Write(buffer, 0, count);
        }

        /// <summary> 写post参数 </summary>
        /// <param name="paras"></param>
        private void WriteParams(string paras)
        {
            if (string.IsNullOrWhiteSpace(paras)) return;
            var buffer = _encoding.GetBytes(paras);
            WriteRequestStream(buffer, buffer.Length);
        }

        /// <summary> 写文件 </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        private void WriteFileStream(string name, Stream file)
        {
            var fileField = new StringBuilder();
            fileField.Append(string.Format("{1}--{0}{1}", Boundary, NewLine));
            fileField.Append(string.Format(
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}",
                Path.GetFileNameWithoutExtension(name), Path.GetFileName(name), NewLine));
            //文件类型
            fileField.Append(string.Format("Content-Type: {0}{1}{1}", GetContentType(name), NewLine));
            WriteParams(fileField.ToString());

            var buffer = new byte[checked((uint)Math.Min(4096, (int)file.Length))];
            int bytesRead;
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) != 0)
                WriteRequestStream(buffer, bytesRead);
            WriteParams(NewLine);
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext) || !Constants.ContentTypes.ContainsKey(ext))
                return Constants.ContentTypes["*"];
            return Constants.ContentTypes[ext];
        }

        /// <summary> 添加头文件 </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
        {
            if (_headers.ContainsKey(key))
                _headers[key] = value;
            else
                _headers.Add(key, value);
        }

        public void SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        /// <summary> 添加文件 </summary>
        /// <param name="fileList"></param>
        public void AddFiles(Dictionary<string, Stream> fileList)
        {
            if (_fileList == null)
                _fileList = new Dictionary<string, Stream>();
            foreach (var key in fileList.Keys)
            {
                if (!_fileList.ContainsKey(key))
                    _fileList.Add(key, fileList[key]);
            }
        }

        /// <summary> 添加文件 </summary>
        /// <param name="pathList"></param>
        public void AddFiles(List<string> pathList)
        {
            if (_fileList == null)
                _fileList = new Dictionary<string, Stream>();
            var list =
                pathList.Select(path => new FileStream(path, FileMode.Open, FileAccess.Read)).ToList();
            foreach (var fileStream in list)
            {
                if (!_fileList.ContainsKey(fileStream.Name))
                    _fileList.Add(fileStream.Name, fileStream);
            }
        }

        /// <summary> 获取请求的url </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            return _req?.Address.ToString() ?? Empty;
        }

        public void SetUserAgent(string userAgent)
        {
            _userAgent = userAgent;
        }

        /// <summary> 设置有帐号的代理 </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <param name="ip"></param>
        public void SetWebProxy(string userName, string userPwd, string ip)
        {
            //设置代理服务器
            _proxy = new WebProxy(ip, false)
            {
                //建立连接
                Credentials = new NetworkCredential(userName, userPwd)
            };
        }

        /// <summary> 设置免费代理 </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SetWebProxy(string ip, int port)
        {
            //设置代理服务器
            _proxy = new WebProxy(ip, port);
        }

        /// <summary> 获取返回流 </summary>
        public Stream GetStream(bool saveCookie = false)
        {
            CreateHttpRequest();
            if (_req == null)
                return null;
            Stream stream = null;
            try
            {
                _rep = (HttpWebResponse)_req.GetResponse();
                if (saveCookie)
                {
                    _cookie = _rep.Headers.Get("Set-Cookie");
                }
                stream = _rep.GetResponseStream();
                if (stream == null)
                    return null;
                if (_rep.ContentEncoding.ToLower() == "gzip")
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return stream;
        }

        /// <summary> 执行请求 </summary>
        public string Request(bool saveCookie = false)
        {
            using (var stream = GetStream(saveCookie))
            {
                if (stream == null)
                    return string.Empty;
                using (var sr = new StreamReader(stream, _encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary> 执行请求 </summary>
        public T Request<T>(bool saveCookie = false)
        {
            var html = Request(saveCookie);
            if (string.IsNullOrWhiteSpace(html))
                return default(T);
            return JsonHelper.Json<T>(html);
        }

        /// <summary> 下载文件 </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Download(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            else
            {
                return false;
            }
            using (var resStream = GetStream())
            {
                if (resStream == null)
                    return false;
                try
                {
                    using (Stream fileStream = new FileStream(path, FileMode.Create))
                    {
                        var by = new byte[1024];
                        var osize = resStream.Read(by, 0, by.Length);
                        while (osize > 0)
                        {
                            fileStream.Write(by, 0, osize);
                            osize = resStream.Read(by, 0, by.Length);
                        }
                        resStream.Close();
                        fileStream.Close();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            if (_fileList != null && _fileList.Any())
            {
                foreach (var fileStream in _fileList)
                {
                    fileStream.Value.Close();
                    fileStream.Value.Dispose();
                }
            }
            if (_postStream != null)
            {
                _postStream.Close();
                _postStream.Dispose();
            }
            _rep?.Close();
            _req?.Abort();
        }

        #endregion
    }
}
