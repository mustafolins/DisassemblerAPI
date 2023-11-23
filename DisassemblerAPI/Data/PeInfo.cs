namespace DisassemblerAPI.Data
{
    public class PeInfo
    {
        public string ErrorMessage { get; set; }
        public bool LoadedImage { get; set; }
        public bool EntierImageAvailable { get; set; }
        public List<SectionInfo> Sections { get; set; }
    }
}
