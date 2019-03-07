using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AOP.Netcore.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace TestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IValues _iValues;

        public ValuesController(IValues values)
        {
            _iValues = values;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _iValues.getMany();
        }

        // GET api/values/5
        [Cached(30)]
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return _iValues.getOne();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
