using System.Reflection;

namespace NivoMaxBot.Messaging.Routing
{
    public class RouteEntry
    {
        public RouteTemplate Template { get; set; }
        public Type HandlerType { get; set; }
        public MethodInfo HandleMethod { get; set; }
    }
}
