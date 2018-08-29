
using System;
using System.Globalization;
using System.Net;
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
        private const string CompaniesEndpoint = "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/companies";
        private const string ExpectedCompanyName = "test";

        [SetUp]
        public void Initialize()
        {
            _sut = new RESTHelper();
            _client = new RestClient(CompaniesEndpoint);
            if (_sut.DoesAnyEntityExist(_client)) _sut.DeleteAllEntities(_client);

        }

        [TearDown]
        public void Dispose()
        {
            _sut.DeleteAllEntities(_client);
            _sut = null;
            _client = null;
        }

        [Test]
        public void CompanyCreationIsPossible()
        {
            var response = _sut.CreateEntityAndGetAStatusCode(_client, "test" + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CompanyCreationWithInvalidAccessTokenIsNotAllowed()
        {
            _sut = new RESTHelper(false);
            var response = _sut.CreateEntityAndGetAStatusCode(_client, "test" + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void GetAllExistingCompaniesIsPossible()
        {
            CreateANumberOfCompanies(3);
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetCompanyByValidIdIsPossible()
        {
            CreateANumberOfCompanies();
            var ids = _sut.GetActualIds(_client);
            var response = _sut.GetEntityById(_client, ids[0]);
            var companyById = JsonConvert.DeserializeObject<EntityModel>(response.Content);

            StringAssert.StartsWith(ExpectedCompanyName, companyById.Name);
        }

        [Test]
        public void GetCompanyByInvalidIdThrowsArgumentOutOfRangeException()
        {
            CreateANumberOfCompanies();
            var ids = _sut.GetActualIds(_client);

            Assert.That(() => _sut.GetEntityById(_client, ids[1]), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void DeleteCompanyByIdIsPossible()
        {
            CreateANumberOfCompanies(3);
            var ids = _sut.GetActualIds(_client);
            var response = _sut.DeleteEntityById(_client, ids[2]);
            var newIds = _sut.GetActualIds(_client);

            Assert.That(response.StatusCode.ToString(), Is.EqualTo("OK"));
            Assert.That(newIds, Has.No.Member(response));
        }

        /// <summary>
        /// Creates a specified number of companies and resets Entity Code via resetting
        /// </summary>
        /// <param name="numberOfCompaniesToCreate">number of companies that will be created. By default creates one company</param>
        private void CreateANumberOfCompanies(int numberOfCompaniesToCreate=1)
        {
            while (numberOfCompaniesToCreate > 0)
            {
                _sut.CreateEntity(_client, "test" + _sut.EntityCode);
                _sut.EntityCode = Guid.NewGuid().GetHashCode().ToString(CultureInfo.InvariantCulture);
                numberOfCompaniesToCreate -= 1;
            }
        }
    }
}
