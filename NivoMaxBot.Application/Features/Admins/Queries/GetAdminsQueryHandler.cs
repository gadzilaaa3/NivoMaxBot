using MediatR;
using NivoMaxBot.Application.Features.Admins.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Admins.Queries
{
    public class GetAdminsQueryHandler : IRequestHandler<GetAdminsQuery, IEnumerable<AdminDto>>
    {
        private readonly IAdminRepository _adminRepository;

        public GetAdminsQueryHandler(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<AdminDto>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        {
            var admins = await _adminRepository.GetAllAsync(cancellationToken);
            return admins.Select(a => new AdminDto
            {
                Id = a.Id,
                MaxId = a.MaxId,
                Username = a.Username,
                IsSuperAdmin = a.IsSuperAdmin
            }).ToList();
        }
    }
}
