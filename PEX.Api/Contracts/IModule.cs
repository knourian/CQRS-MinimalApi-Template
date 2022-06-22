namespace PEX.Application.Contracts;
public interface IModule
{
    IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints);
}
