using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new System.ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<List<OrderDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var orderListByUserName = await _orderRepository.GetOrdersByUserName(request.UserName);
            var orderDto = _mapper.Map<List<OrderDto>>(orderListByUserName);
            return orderDto;
        }
    }

    public class GetOrderListQuery : IRequest<List<OrderDto>>
    {
        public GetOrderListQuery(string userName)
        {
            UserName = userName ?? throw new System.ArgumentNullException(nameof(userName));
        }

        public string UserName { get; }
    }
}
