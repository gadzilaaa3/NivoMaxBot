namespace NivoMaxBot.Application.Common
{
    public class PendingAction
    {
        public string ActionType { get; set; } = ""; // например "AddToCart"
        public Dictionary<string, object> Parameters { get; set; } = []; // параметры действия (productId и т.п.)
        public string? ReturnCallback { get; set; } // куда вернуться после выполнения (опционально)
    }
}
