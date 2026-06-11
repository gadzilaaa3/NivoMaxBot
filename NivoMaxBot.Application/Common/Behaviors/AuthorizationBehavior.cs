using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Attributes;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Interfaces.Repositories;
using System.Reflection;

namespace NivoMaxBot.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

        public AuthorizationBehavior(ICurrentUserService currentUser, 
            IAdminRepository adminRepository, 
            ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
        {
            _currentUser = currentUser;
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttribute = typeof(TRequest).GetCustomAttribute<AuthorizeAttribute>();
            if (authorizeAttribute == null)
                return await next(); // атрибут не требуется, пропускаем

            if (!_currentUser.IsAuthenticated)
                throw new UnauthorizedAccessException("Пользователь не аутентифицирован");

            var admin = await _adminRepository.GetByMaxIdAsync(_currentUser.MaxId.Value, cancellationToken);
            if (admin == null)
                throw new UnauthorizedAccessException("Требуются права администратора");

            if (authorizeAttribute.RequiredRole == AdminRole.SuperAdmin && !admin.IsSuperAdmin)
                throw new UnauthorizedAccessException("Требуются права суперадминистратора");

            return await next();
        }
    }
}
