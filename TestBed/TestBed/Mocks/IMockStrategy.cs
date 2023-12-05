using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace TestBed.Mocks;

public interface IMockStrategy
{
    Type GetMockType();

    object GetMock();

    void RegisterMockService(IServiceCollection services);
}
