using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Products.Commands.Delete
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IOrderRepository _orderRepository;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            IBasketRepository basketRepository,
            IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _basketRepository = basketRepository;
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.Id);

            // Проверка использования в корзинах
            var inBasket = await _basketRepository.IsProductInAnyBasketAsync(request.Id, cancellationToken);
            if (inBasket)
                throw new BusinessRuleViolationException("Нельзя удалить товар, который находится в корзинах пользователей.");

            // Проверка использования в заказах
            var inOrder = await _orderRepository.IsProductInAnyOrderAsync(request.Id, cancellationToken);
            if (inOrder)
                throw new BusinessRuleViolationException("Нельзя удалить товар, который присутствует в заказах.");

            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
