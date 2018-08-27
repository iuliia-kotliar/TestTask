
using API_Assessment.Helpers;
using API_Assessment.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace API_Assessment.Tests
{
    [TestFixture()]
    public class CompanyTests
    {
        private readonly RestClient _client = new RestClient("https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/companies");

        [Test]
        public void CreateCompany()
        {
            RESTHelper token = new RESTHelper();
            var response = token.CreateEntity(_client, "test"+ token.EntityCode);
            Assert.That(response, Is.EqualTo("OK"));
        }

        [Test]
        public void GetAllCompanies()
        {
            RESTHelper token = new RESTHelper();
            var response = token.GetAllEntities(_client);
            var parsed = token.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetCompanyById()
        {
            RESTHelper token = new RESTHelper();
            var response = token.GetEntityById(_client, 1);
            var expectedResponse = "test";
            var companyById = JsonConvert.DeserializeObject<EntityModel>(response.Content);
            StringAssert.StartsWith(expectedResponse, companyById.Name);
        }
    }
}
