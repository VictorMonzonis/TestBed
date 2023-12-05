using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TestBed.Mocks;

namespace TestBedXunitTests.Mocks;

public class MyMockExampleA : IMockStrategy
{
    ICustomerAccount _customerAccount = Substitute.For<ICustomerAccount>();

    public object GetMock() => _customerAccount;

    public Type GetMockType() => _customerAccount.GetType();

    public void RegisterMockService(IServiceCollection services)
    {
        services.AddSingleton<ICustomerAccount>(_ => _customerAccount);
    }

    public MyMockExampleA()
    {
        DefaultBehaviours();
    }

    private void DefaultBehaviours()
    {
        _customerAccount.GetCustomerName(Arg.Any<int>()).Returns("Test");
    }
}

public interface ICustomerAccount
{
    string GetCustomerName(int id);
}
