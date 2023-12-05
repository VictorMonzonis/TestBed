using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using TestBed;
using TestBedXunitTests.Mocks;
using Xunit.Abstractions;

namespace TestBedXunitTests
{
    public class OffspringTest : IClassFixture<TestBed<Program>>
    {
        private readonly TestBed<Program> _testBed;
        private readonly ITestOutputHelper _output;

        private ICustomerAccount _customerAccountService;

        public OffspringTest(TestBed<Program> testBed, ITestOutputHelper output)
        {
            _output = output;
            _testBed = testBed;
            _testBed.MockServices = new[] { new MyMockExampleA() }; // register the mocks
            _testBed.OneTimeSetupHandleCallback = () => 
            {
                _output.WriteLine("Delete the DB");
            };
            _testBed.Initializer();

            _customerAccountService = _testBed.Services!.GetRequiredService<ICustomerAccount>();
        }

        [Fact]
        public void TestBed_loadMocks_defaultInfo()
        {
            var name = _customerAccountService.GetCustomerName(1);

            Assert.Equal("Test", name);
        }

        [Fact]
        public void TestBed_loadMocks_OverwrittenInfo()
        {
            _customerAccountService.GetCustomerName(2).Returns("NameNumber2");

            var name = _customerAccountService.GetCustomerName(2);

            Assert.Equal("NameNumber2", name);
        }
    }
}