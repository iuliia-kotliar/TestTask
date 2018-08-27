
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
        private RestClient _client;
        private RESTHelper _sut;

        [SetUp]
        public void Initialize()
        {
            _sut = new RESTHelper();
            _client = new RestClient("https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/companies");
        }

        [TearDown]
        public void Dispose()
        {
            _sut = null;
            _client = null;
        }

        [Test]
        public void CreateCompany()
        {
            var response = _sut.CreateEntity(_client, "test"+ _sut.EntityCode);
            Assert.That(response, Is.EqualTo("OK"));
        }

        [Test]
        public void GetAllCompanies()
        {
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetCompanyById()
        {
            var response = _sut.GetEntityById(_client, 2);
            var expectedResponse = "test";
            var companyById = JsonConvert.DeserializeObject<EntityModel>(response.Content);
            StringAssert.StartsWith(expectedResponse, companyById.Name);
        }

        [Test]
        public void DeleteCompanyById()
        {
            var response = _sut.DeleteEntityById(_client, 3);
            Assert.That(response.StatusCode.ToString(), Is.EqualTo("OK"));
        }
    }
}
