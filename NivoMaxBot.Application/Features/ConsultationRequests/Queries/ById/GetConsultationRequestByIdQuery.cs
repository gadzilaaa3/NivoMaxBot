using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Queries.ById
{
    public class GetConsultationRequestByIdQuery 
        : IRequest<ConsultationRequestDto>
    {
        public int Id { get; set; }
    }
}
