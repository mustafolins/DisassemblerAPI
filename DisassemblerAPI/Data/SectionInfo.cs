using Iced.Intel;

namespace DisassemblerAPI.Data
{
    public class SectionInfo
    {
        public string Name { get; set; }
        public string SectionCharacteristics { get; set; }
        public int Length { get; set; }
        public List<string> Instructions { get; set; }
    }
}
