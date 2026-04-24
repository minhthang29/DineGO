/// <summary>
/// Represents the settings required for API configuration.
/// </summary>
namespace Core.Common
{
    public class ApiSettings
    {
        /// <summary>
        /// Gets or sets the base URL for the API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the domain for the API.
        /// </summary>
        public string ApiDomain { get; set; }
    }
}