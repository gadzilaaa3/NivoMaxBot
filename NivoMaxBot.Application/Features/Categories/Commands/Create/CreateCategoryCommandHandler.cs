using MediatR;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, 
            IAdminRepository adminRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name,
                ParentId = request.ParentId,
                Order = request.Order
            };
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return category.Id;
        }
    }
}
