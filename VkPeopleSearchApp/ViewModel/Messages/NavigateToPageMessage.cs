using System.Collections.Generic;

namespace VkPeopleSearchApp.ViewModel.Messages
{
    /// <summary>
    /// Uses for navigation to page
    /// </summary>
    public class NavigateToPageMessage
    {
        /// <summary>
        /// Page
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// Params
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }


        /// <summary>
        /// Clear history after navigating
        /// </summary>
        public bool ClearHistory { get; set; }
    }
}
