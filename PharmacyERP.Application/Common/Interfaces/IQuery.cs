using MediatR;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}