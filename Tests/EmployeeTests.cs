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
    [TestFixture]
    public class EmployeeTests
    {
        private RestClient _client;
        private RESTHelper _sut;

        private const string EmployeesEndpoint =
            "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/employees";
        private const string ExpectedEmployeeName = "test";
        private const string NewEmployeeName = "available";
        private const string NumberOfExistingEmployeesErrorMessage =
            "Expected number of existing employees difffers from the actual one.";

        [SetUp]
        public void Initialize()
        {
            _sut = new RESTHelper();
            _client = new RestClient(EmployeesEndpoint);
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
        public void EmployeeCreationReturnsOkStatusCode()
        {
            var response = _sut.CreateEntityAndGetAStatusCode(_client, ExpectedEmployeeName + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.OK),
                "Employee creation did not result in the 'Ok' status code, but should have.");
        }


        [Test]
        public void CreatedEmployeeIsAfterwardsAvailbleInExistingEmployees()
        {
            _sut.CreateEntity(_client, NewEmployeeName + _sut.EntityCode);
            var employeeNames = _sut.GetActualNames(_client);

            Assert.That(employeeNames, Has.Some.Contains(NewEmployeeName),
                "Created Employee name is not present in the actual names, but should be.");
        }

        [Test]
        public void CreatingEmployeeWithTheSameNameIsNotAllowed()
        {
            var codes = CreateDuplicateEmployees();

            Assert.That(codes[1], Is.EqualTo(HttpStatusCode.BadRequest),
                "Creating Employees with duplicate names was allowed, but should not have been.");
        }

        [Test]
        public void EmployeeCreationWithInvalidAccessTokenIsNotAllowed()
        {
            _sut = new RESTHelper(false);
            var response = _sut.CreateEntityAndGetAStatusCode(_client, ExpectedEmployeeName + _sut.EntityCode);

            Assert.That(response, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Employee creation with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void GetAllExistingEmployeesIsPossible()
        {
            CreateANumberOfEmployees(3);
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(3), NumberOfExistingEmployeesErrorMessage);
        }

        [Test]
        public void GetAllExistingEmployeesWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfEmployees();
            _sut = new RESTHelper(false);
            var response = _sut.GetAllEntities(_client);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Get all existing Employees with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void RunningGetAllWhenNoEmployeeWasCreatedReturnsEmptyResponse()
        {
            var response = _sut.GetAllEntities(_client);
            var parsed = _sut.ParseEntityResponse(response);

            Assert.That(parsed.Count, Is.EqualTo(0), NumberOfExistingEmployeesErrorMessage);
        }

        [Test]
        public void GetEmployeeByValidIdIsPossible()
        {
            CreateANumberOfEmployees();
            var ids = _sut.GetActualIds(_client);
            var response = _sut.GetEntityById(_client, ids[0]);
            var employeeById = JsonConvert.DeserializeObject<EntityModel>(response.Content);

            StringAssert.StartsWith(ExpectedEmployeeName, employeeById.Name, "Expected Employee name differs from the actual one.");
        }

        [Test]
        public void GetEmployeeByInvalidIdThrowsArgumentOutOfRangeException()
        {
            CreateANumberOfEmployees();
            var ids = _sut.GetActualIds(_client);

            Assert.That(() => _sut.GetEntityById(_client, ids[1]), Throws.TypeOf<ArgumentOutOfRangeException>(),
                "ArgumentOutOfRangeException was not thrown when trying to access item in the empty collection.");
        }

        [Test]
        public void GetByIdWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfEmployees();
            var ids = _sut.GetActualIds(_client);
            _sut = new RESTHelper(false);
            var response = _sut.GetEntityById(_client, ids[0]);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Get by ID with the invalid token was allowed, but should not have been.");
        }

        [Test]
        public void DeleteEmployeeByIdIsPossible()
        {
            CreateANumberOfEmployees(3);
            var ids = _sut.GetActualIds(_client);
            var response = _sut.DeleteEntityById(_client, ids[2]);
            var newIds = _sut.GetActualIds(_client);

            Assert.That(response.StatusCode.ToString(), Is.EqualTo("OK"), "Delete operation did not result in the 'Ok' status code.");
            Assert.That(newIds, Has.No.Member(response),
                "Deleted Employee was present in the list of Employee ids, but should not have been.");
        }

        [Test]
        public void DeleteEmployeeWithInvalidTokenIsNotAllowed()
        {
            CreateANumberOfEmployees(3);
            var ids = _sut.GetActualIds(_client);
            _sut = new RESTHelper(false);
            var response = _sut.DeleteEntityById(_client, ids[2]);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                "Delete Employee with the invalid token was allowed, but should not have been.");
        }

        /// <summary>
        /// Creates a specified number of Employees and resets Entity Code via resetting
        /// </summary>
        /// <param name="numberOfEmployeesToCreate">number of Employees that will be created. By default creates one Employee</param>
        private void CreateANumberOfEmployees(int numberOfEmployeesToCreate = 1)
        {
            while (numberOfEmployeesToCreate > 0)
            {
                _sut.CreateEntity(_client, ExpectedEmployeeName + _sut.EntityCode);
                _sut.EntityCode = Guid.NewGuid().GetHashCode().ToString(CultureInfo.InvariantCulture);
                numberOfEmployeesToCreate -= 1;
            }
        }

        /// <summary>
        /// Tries creating two Employee with the same name
        /// </summary>
        /// <returns>List of Http status codes</returns>
        private List<HttpStatusCode> CreateDuplicateEmployees()
        {
            var numberOfEmployeesToCreate = 2;
            var statusCodes = new List<HttpStatusCode>();
            while (numberOfEmployeesToCreate > 0)
            {
                statusCodes.Add(_sut.CreateEntityAndGetAStatusCode(_client, ExpectedEmployeeName + _sut.EntityCode));
                numberOfEmployeesToCreate -= 1;
            }
            return statusCodes;
        }
    }
}
