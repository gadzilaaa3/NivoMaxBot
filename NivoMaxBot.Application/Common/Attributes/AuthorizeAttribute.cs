namespace NivoMaxBot.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AuthorizeAttribute : Attribute
    {
        public AdminRole RequiredRole { get; set; } = AdminRole.Admin;
    }
}
