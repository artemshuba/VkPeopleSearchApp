using System.Collections.Generic;
using System.Text;
using VkLib.Core.Users;
using VkLib.Core.Users.Types;
using VkPeopleSearchApp.Helpers;

namespace VkPeopleSearchApp.Model
{
    public class SearchParams
    {
        /// <summary>
        /// Country
        /// </summary>
        public VkCountry Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public VkCity City { get; set; }

        /// <summary>
        /// Sex
        /// </summary>
        public VkUserSex Sex { get; set; }

        /// <summary>
        /// Minimal age
        /// </summary>
        public int AgeMin { get; set; }

        /// <summary>
        /// Maximal age
        /// </summary>
        public int AgeMax { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public VkUserStatus? Status { get; set; }

        /// <summary>
        /// Indicates that params changed and we need to reload search results
        /// </summary>
        public bool NeedReload { get; set; }

        public override string ToString()
        {
            var p = new List<string>();

            if (Country != null)
                p.Add(Country.Title);

            if (City != null)
                p.Add(City.Title);

            if (Sex == VkUserSex.Male)
                p.Add("Мужской");
            else if (Sex == VkUserSex.Female)
                p.Add("Женский");

            if (AgeMin != 0 && AgeMax != 0)
            {
                if (AgeMin != AgeMax)
                {
                    p.Add(string.Format("от {0} до {1} лет", AgeMin, AgeMax));
                }
                else
                    p.Add(AgeMin + " " + StringHelper.LocNum(AgeMin, "год", "года", "лет"));
            }

            if (Status != null)
            {
                p.Add(StringHelper.GetStatusString(Status.Value, Sex));
            }

            return string.Join(", ", p);
        }
    }
}
