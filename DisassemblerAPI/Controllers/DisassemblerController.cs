using DisassemblerAPI.Data;
using Iced.Intel;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DisassemblerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisassemblerController : ControllerBase
    {
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

        // POST api/<DisassemblerController>/<PEInfo>
        [Route("[action]")]
        [HttpPost]
        public PeInfo PEInfo([FromBody] string base64String)
        {
            using var stream = new MemoryStream(Convert.FromBase64String(base64String));
            using var pEReader = new PEReader(stream);

            // initialize response object
            var peInfo = new PeInfo
            {
                LoadedImage = pEReader.IsLoadedImage,
                EntierImageAvailable = pEReader.IsEntireImageAvailable
            };

            try
            {
                // get the different section's information
                var sectionInformations = new List<SectionInfo>();
                foreach (var sectionHeader in pEReader.PEHeaders.SectionHeaders)
                {
                    // get section data from pe reader
                    var memoryBlock = pEReader.GetSectionData(sectionHeader.Name);

                    var sectionInfo = new SectionInfo
                    {
                        Name = sectionHeader.Name,
                        SectionCharacteristics = sectionHeader.SectionCharacteristics.ToString(),
                        Length = memoryBlock.Length
                    };

                    // get bytes from section
                    var blobReader = memoryBlock.GetReader();
                    var bytes = blobReader.ReadBytes(memoryBlock.Length);

                    // initialize x86 decoder with section bytes
                    using var sectionStream = new MemoryStream(bytes);
                    var reader = new StreamCodeReader(sectionStream);
                    var decoder = Decoder.Create(64, reader);

                    // get instructions
                    var instructions = new List<string>();
                    while (sectionStream.Position < sectionStream.Length) // don't worry stream will be incremented by the decoder
                    {
                        instructions.Add($"{decoder.Decode()}");
                    }

                    sectionInfo.Instructions = instructions;
                    sectionInformations.Add(sectionInfo);
                }
                peInfo.Sections = sectionInformations;
            }
            catch (Exception ex)
            {
                peInfo.ErrorMessage = ex.Message;
            }

            return peInfo;
        }
    }
}
