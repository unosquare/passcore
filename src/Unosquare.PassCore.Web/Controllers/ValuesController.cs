using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Unosquare.PassCore.Web.Models;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Unosquare.PassCore.Web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {

        public ValuesController(IConfigurationRoot configuration)
            : base(configuration)
        {
            // placeholder
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { Settings.ActiveDirectoryRootPath, Configuration.Get<string>("AppSettings:ActiveDirectoryRootPath") };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
