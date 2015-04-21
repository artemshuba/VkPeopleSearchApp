// This is a small subset of VkLib library. Full version can be found here https://github.com/Stealth2012/VkLib

using System.Collections.Generic;
using VkLib.Core.Auth;
using VkLib.Core.Database;
using VkLib.Core.Users;

namespace VkLib
{
    /// <summary>
    /// Core object for data access
    /// </summary>
    public class Vkontakte
    {
        private readonly string _clientSecret;
        private readonly string _appId;
        private string _apiVersion = "5.29";

        internal string AppId
        {
            get { return _appId; }
        }

        internal string ClientSecret
        {
            get { return _clientSecret; }
        }

        /// <summary>
        /// Api version
        /// </summary>
        public string ApiVersion
        {
            get { return _apiVersion; }
            set { _apiVersion = value; }
        }

        /// <summary>
        /// Access token
        /// </summary>
        public AccessToken AccessToken { get; set; }


        /// <summary>
        /// Users
        /// </summary>
        public VkUsersRequest Users
        {
            get
            {
                return new VkUsersRequest(this);
            }
        }

        /// <summary>
        /// OAuth
        /// </summary>
        public VkOAuthRequest OAuth
        {
            get
            {
                return new VkOAuthRequest(this);
            }
        }


        /// <summary>
        /// Direct Auth by login and password
        /// </summary>
        public VkDirectAuthRequest Auth
        {
            get
            {
                return new VkDirectAuthRequest(this);
            }
        }

        /// <summary>
        /// Database
        /// </summary>
        public VkDatabaseRequest Database
        {
            get
            {
                return new VkDatabaseRequest(this);
            }
        }


        public Vkontakte(string appId, string clientSecret = null, string apiVersion = null)
        {
            AccessToken = new AccessToken();

            if (apiVersion != null)
                ApiVersion = apiVersion;

            _appId = appId;
            _clientSecret = clientSecret;
        }

        internal void SignMethod(Dictionary<string, string> parameters)
        {
            if (parameters == null)
                parameters = new Dictionary<string, string>();

            parameters.Add("access_token", AccessToken.Token);

            if (!string.IsNullOrEmpty(ApiVersion))
                parameters.Add("v", ApiVersion);
        }
    }
}
