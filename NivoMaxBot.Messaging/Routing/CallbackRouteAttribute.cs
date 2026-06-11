namespace NivoMaxBot.Messaging.Routing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CallbackRouteAttribute : Attribute
    {
        public string Template { get; }
        public CallbackRouteAttribute(string template) => Template = template;
    }
}
