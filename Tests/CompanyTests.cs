
using System;
using System.Collections.Generic;
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

        private const string CompaniesEndpoint =
            "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/companies";
        private const string ExpectedCompanyName = "test";
        private const string NewCompanyName = "available";
        private const string NumberOfExistingCompaniesErrorMessage = 
            "Expected number of existing companies difffers from the actual one.";

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
        public void CompanyCreationReturnsOkStatusCode()
        {
            var response = _sut.CreateEntityAndGetAStatusCode(_client, ExpectedCompanyName + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.OK), 
                "Company creation did not result in the 'Ok' status code, but should have.");
        }


        [Test]
        public void CreatedCompanyIsAfterwardsAvailbleInExistingCompanies()
        {
            _sut.CreateEntity(_client, NewCompanyName + _sut.EntityCode);
            var companyNames = _sut.GetActualNames(_client);

            Assert.That(companyNames, Has.Some.Contains(NewCompanyName), 
                "Created company name is not present in the actual names, but should be.");
        }

        [Test]
        public void CreatingCompanyWithTheSameNameIsNotAllowed()
        {
            var codes = CreateDuplicateCompanies();

            Assert.That(codes[1], Is.EqualTo(HttpStatusCode.BadRequest), 
                "Creating companies with duplicate names was allowed, but should not have been.");
        }

        [Test]
        public void CompanyCreationWithInvalidAccessTokenIsNotAllowed()
        {
            _sut = new RESTHelper(false);
            var response = _sut.CreateEntityAndGetAStatusCode(_client, ExpectedCompanyName + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.Unauthorized), 
                "Company creation with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void GetAllExistingCompaniesIsPossible()
        {
            CreateANumberOfCompanies(3);
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(3), NumberOfExistingCompaniesErrorMessage);
        }

        [Test]
        public void GetAllExistingCompaniesWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfCompanies();
            _sut = new RESTHelper(false);
            var response = _sut.GetAllEntities(_client);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), 
                "Get all existing companies with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void RunningGetAllWhenNoCompanyWasCreatedReturnsEmptyResponse()
        {
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(0), NumberOfExistingCompaniesErrorMessage);
        }

        [Test]
        public void GetCompanyByValidIdIsPossible()
        {
            CreateANumberOfCompanies();
            var ids = _sut.GetActualIds(_client);
            var response = _sut.GetEntityById(_client, ids[0]);
            var companyById = JsonConvert.DeserializeObject<EntityModel>(response.Content);

            StringAssert.StartsWith(ExpectedCompanyName, companyById.Name, "Expected company name differs from the actual one.");
        }

        [Test]
        public void GetCompanyByInvalidIdThrowsArgumentOutOfRangeException()
        {
            CreateANumberOfCompanies();
            var ids = _sut.GetActualIds(_client);

            Assert.That(() => _sut.GetEntityById(_client, ids[1]), Throws.TypeOf<ArgumentOutOfRangeException>(), 
                "ArgumentOutOfRangeException was not thrown when trying to access item in the empty collection.");
        }

        [Test]
        public void GetByIdWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfCompanies();
            var ids = _sut.GetActualIds(_client);
            _sut = new RESTHelper(false);
            var response = _sut.GetEntityById(_client, ids[0]);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Get by ID with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void DeleteCompanyByIdIsPossible()
        {
            CreateANumberOfCompanies(3);
            var ids = _sut.GetActualIds(_client);
            var response = _sut.DeleteEntityById(_client, ids[2]);
            var newIds = _sut.GetActualIds(_client);

            Assert.That(response.StatusCode.ToString(), Is.EqualTo("OK"), "Delete operation did not result in the 'Ok' status code.");
            Assert.That(newIds, Has.No.Member(response), 
                "Deleted company was present in the list of company ids, but should not have been.");
        }

        [Test]
        public void DeleteCompanyWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfCompanies(3);
            var ids = _sut.GetActualIds(_client);
            _sut = new RESTHelper(false);
            var response = _sut.DeleteEntityById(_client, ids[2]);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Delete company with the invalid token was allowed, but should not have been.");
        }

        /// <summary>
        /// Creates a specified number of companies and resets Entity Code via resetting
        /// </summary>
        /// <param name="numberOfCompaniesToCreate">number of companies that will be created. By default creates one company</param>
        private void CreateANumberOfCompanies(int numberOfCompaniesToCreate=1)
        {
            while (numberOfCompaniesToCreate > 0)
            {
                _sut.CreateEntity(_client, ExpectedCompanyName + _sut.EntityCode);
                _sut.EntityCode = Guid.NewGuid().GetHashCode().ToString(CultureInfo.InvariantCulture);
                numberOfCompaniesToCreate -= 1;
            }
        }

        /// <summary>
        /// Tries creating two company with the same name
        /// </summary>
        /// <returns>List of Http status codes</returns>
        private List<HttpStatusCode> CreateDuplicateCompanies()
        {
            var numberOfCompaniesToCreate = 2;
            var statusCodes = new List<HttpStatusCode>();
            while (numberOfCompaniesToCreate > 0)
            {
                statusCodes.Add(_sut.CreateEntityAndGetAStatusCode(_client, ExpectedCompanyName + _sut.EntityCode));
                numberOfCompaniesToCreate -= 1;
            }
            return statusCodes;
        }
    }
}
