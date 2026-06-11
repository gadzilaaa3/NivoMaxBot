using MediatR;
using NivoMaxBot.Application.Features.Admins.Dtos;

namespace NivoMaxBot.Application.Features.Admins.Queries
{
    public class GetAdminsQuery : IRequest<IEnumerable<AdminDto>> { }
}
