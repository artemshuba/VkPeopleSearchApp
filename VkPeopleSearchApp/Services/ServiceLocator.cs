using VkLib;

namespace VkPeopleSearchApp.Services
{
    public class ServiceLocator
    {
        private static Vkontakte _vk = new Vkontakte("4860005", "kMri0XOolj5KfpsR6ehg");

        public static Vkontakte Vkontakte
        {
            get { return _vk; }
        }
    }
}
