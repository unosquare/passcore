using Newtonsoft.Json.Linq;

namespace Unosquare.PassCore.Web.Models
{
    public class AppSettings
    {
        public string RecaptchaPrivateKey { get; set; }
        public JObject ClientSettings { get; set; }
    }
}
