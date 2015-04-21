using VkLib.Core.Auth;

namespace VkPeopleSearchApp.Domain
{
    public class Settings : AppSettings
    {
        private static readonly Settings _instance = new Settings();

        public static Settings Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Vk access token
        /// </summary>
        public AccessToken AccessToken { get; set; }

        public override void Load()
        {
            AccessToken = Get<AccessToken>("AccessToken", null);
        }

        public override void Save()
        {
            Set("AccessToken", AccessToken);
        }
    }
}
