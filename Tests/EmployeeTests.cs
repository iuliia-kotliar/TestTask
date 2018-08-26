using API_Assessment.Helpers;
using NUnit.Framework;
using RestSharp;

namespace API_Assessment.Tests
{
    [TestFixture]
    public class EmployeeTests
    {
        private readonly RestClient _client = new RestClient("https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/employees");

        [Test]
        public void CreateEmployee()
        {
            RESTHelper token = new RESTHelper();
            var response = token.CreateEntity(_client, "Jane" + token.EntityCode);
            Assert.That(response, Is.EqualTo("OK"));
        }
    }
}
