namespace NivoMaxBot.Application.Interfaces
{
    public interface ICurrentUserService
    {
        long? MaxId { get; }
        bool IsAuthenticated { get; }
        void SetUser(long maxId);
        void Clear();
    }
}
