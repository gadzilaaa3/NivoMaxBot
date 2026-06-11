namespace NivoMaxBot.Shared.Brand
{
    public class BrandData
    {
        public BrandItem AboutBrand { get; set; } = new();
        public BrandItem AboutProduction { get; set; } = new();
        public AdvantagesData Advantages { get; set; } = new();
        public string WebsiteUrl { get; set; } = string.Empty;
        public string DealersUrl { get; set; } = string.Empty;
    }
}
