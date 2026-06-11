namespace NivoMaxBot.Application.Common
{
    public class UserState
    {
        public string CurrentAction { get; set; } = string.Empty; // "AddingCategory", "EditingProduct", etc.

        public int? EntityId { get; set; } // ID редактируемой сущности

        public Dictionary<string, object> Data { get; set; } = [];

        public object TypedData { get; set; } = new();

        public int Step { get; set; }

        public PendingAction? PendingAction { get; set; }
    }
}
