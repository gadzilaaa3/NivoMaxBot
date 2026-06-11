using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.ById
{
    public class GetWarrantyRequestByIdQuery : IRequest<WarrantyRequestDto>
    {
        public int Id { get; set; }
    }
}
