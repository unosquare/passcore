using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unosquare.PassCore.Web.Models;

namespace Unosquare.PassCore.Web.Controllers
{
    abstract public class ControllerBase : Controller
    {
        private readonly AppSettings m_Settings;
        private readonly IConfigurationRoot m_Configuration;

        public ControllerBase(IConfigurationRoot configuration)
        {
            m_Settings = new AppSettings();
            ConfigurationBinder.Bind(configuration.GetSection("AppSettings"), m_Settings);
            m_Configuration = configuration;
        }

        protected AppSettings Settings => m_Settings;
        protected IConfigurationRoot Configuration => m_Configuration;
    }
}
