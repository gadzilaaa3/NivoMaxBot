using MediatR;
using NivoMaxBot.Application.Common.Attributes;
using NivoMaxBot.Application.Features.Broadcast.Dtos;

namespace NivoMaxBot.Application.Features.Broadcast.Commands
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class SendBroadcastCommand : IRequest<BroadcastResult>
    {
        public long AdminChatId { get; set; }
        public long SourceChatId { get; set; }
        public string SourceMessageId { get; set; } = string.Empty;
        public BroadcastType BroadcastType { get; set; } = BroadcastType.ActiveUsers;
    }

    public enum BroadcastType
    {
        All,
        ActiveUsers
    }
}
