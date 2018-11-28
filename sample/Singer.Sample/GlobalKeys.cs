namespace ESignature.Business
{
    public class GlobalKeys
    {
        /// <summary> 数据库版本 </summary>
        public const string DbVersion = "db_version";

        public const string Host = "rest_host";

        /// <summary> 登录凭证 </summary>
        public const string AccessToken = "access_token";

        public const string ClientSecret = "client_secret";
        public const string Menus = "system_menus";
        public const string LoginUser = "login_user";
        /// <summary> 机器码 </summary>
        public const string MachineCode = "machine_code";

        public const string VersionUpdateSql = "version_update_sql";

        public const string ApiUserInfo = "api/v1/user/info";
        public const string ApiManifest = "api/v1/common/manifest";
        /// <summary> 注销登录 </summary>
        public const string ApiLogout = "api/v1/account/logout";
    }
}
