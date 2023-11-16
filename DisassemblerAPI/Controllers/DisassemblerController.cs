using Iced.Intel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DisassemblerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisassemblerController : ControllerBase
    {
        // GET: api/<DisassemblerController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DisassemblerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DisassemblerController>/<Instructions>
        [Route("[action]")]
        [HttpPost]
        public async IAsyncEnumerable<Instruction> Instructions([FromBody] string base64String)
        {
            await Task.Delay(1);

            using var stream = new MemoryStream(Convert.FromBase64String(base64String));
            var reader = new StreamCodeReader(stream);
            var decoder = Decoder.Create(64, reader);

            // get instructions
            while (stream.Position < stream.Length) // don't worry stream will be incremented by the decoder
            {
                yield return decoder.Decode();
            }
        }

        // POST api/<DisassemblerController>/<Assembly>
        [Route("[action]")]
        [HttpPost]
        public async IAsyncEnumerable<string> Assembly([FromBody] string base64String)
        {
            await foreach (var instruction in Instructions(base64String))
            {
                yield return $"{instruction}";
            }
        }

        // PUT api/<DisassemblerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DisassemblerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
