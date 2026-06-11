using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public AddToCartCommandHandler(
            IBasketRepository basketRepository, 
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _basketRepository = basketRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null || !product.IsAvailable)
                throw new NotFoundException(nameof(Product), request.ProductId);

            var basket = await _basketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (basket == null)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken) 
                    ?? throw new NotFoundException("Пользователь не существует");

                var initBasket = new Domain.Entities.Basket();
                await _basketRepository.AddAsync(initBasket, cancellationToken);
                await _basketRepository.SaveChangesAsync(cancellationToken);

                user.BasketId = initBasket.Id;
                user.BasketNavigation = initBasket;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync(cancellationToken);

                basket = initBasket;
            }

            var existing = basket.Details.FirstOrDefault(d => d.ProductId == request.ProductId);
            if (existing != null)
            {
                existing.ProductsQuantity += request.Quantity;
                _basketRepository.UpdateDetail(existing);
            }
            else
            {
                var detail = new BasketDetail
                {
                    BasketId = basket.Id,
                    ProductId = request.ProductId,
                    ProductsQuantity = request.Quantity
                };
                basket.Details.Add(detail);
            }

            await _basketRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
