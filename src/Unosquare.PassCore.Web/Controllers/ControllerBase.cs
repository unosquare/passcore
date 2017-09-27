namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Unosquare.PassCore.Web.Models;

    /// <summary>
    /// Represents a base class for controllers.
    /// It provides configuration objects.
    /// </summary>
    abstract public class ControllerBase : Controller
    {
        const string AppSettingsSectionName = "AppSettings";

        private readonly AppSettings m_Settings;
        private readonly IConfigurationRoot m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ControllerBase(IConfigurationRoot configuration)
        {
            m_Settings = new AppSettings();
            ConfigurationBinder.Bind(configuration.GetSection(AppSettingsSectionName), m_Settings);
            m_Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the app settings.
        /// This is not the configuration root node.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected AppSettings Settings => m_Settings;

        /// <summary>
        /// Gets or sets the configuration root node.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        protected IConfigurationRoot Configuration => m_Configuration;
    }
}
