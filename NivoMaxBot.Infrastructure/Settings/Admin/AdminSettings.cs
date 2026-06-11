namespace NivoMaxBot.Infrastructure.Settings.Admin
{
    public class AdminSettings
    {
        public const string SectionName = "AdminSettings";

        public List<InitialAdmin> InitialAdmins { get; set; } = [];
    }
}
