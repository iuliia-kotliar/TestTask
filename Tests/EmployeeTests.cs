using API_Assessment.Helpers;
using NUnit.Framework;
using RestSharp;

namespace API_Assessment.Tests
{
    [TestFixture]
    public class EmployeeTests
    {
        private RestClient _client;
        private RESTHelper _sut;

        [SetUp]
        public void Initialize()
        {
            _sut = new RESTHelper();
            _client = new RestClient("https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/employees");
        }

        [TearDown]
        public void Dispose()
        {
            _sut = null;
            _client = null;
        }
    }
}
