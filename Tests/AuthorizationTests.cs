using System;
using System.Globalization;
using RestSharp;
using API_Assessment.Helpers;
using NUnit.Framework;

namespace API_Assessment.Tests
{
    [TestFixture]
    public class AuthorizationTests
    {
        private RESTHelper _sut;

        [SetUp]
        public void Initialize()
        {
            _sut = new RESTHelper();
        }

        [TearDown]
        public void Dispose()
        {
            _sut = null;
        }

        [Test]
        public void TokenCreationIsSuccessful()
        {
            var response = _sut.CreateToken();
            var code = response.StatusCode;

            Assert.That(code, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        [Test]
        public void IssuedPropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response);
            var toGMT = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Greenwich Standard Time");
            var stringToDateTime = DateTime.ParseExact(parsed.Issued, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            Assert.That(parsed.Issued, Is.EqualTo(toGMT).Within(5).Seconds);
            Assert.That(parsed.Expires, Is.EqualTo(DateTime.Now));
        }

        [Test]
        public void TokenTypePropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).TokenType;

            Assert.That(parsed, Is.EqualTo("bearer"));
        }

        [Test]
        public void ExpiresInPropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).ExpiresIn;

            Assert.That(parsed, Is.EqualTo("299"));
        }

        [Test]
        public void DisplayNamePropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).DisplayName;

            Assert.That(parsed, Is.EqualTo("testName_alias"));
        }

        [Test]
        public void CreateTokenWithEmptyCredentials()
        {
            var response = _sut.CreateToken("", "", "password");
            var code = response.StatusCode.ToString();

            Assert.That(code, Is.EqualTo("OK"));
        }

        [Test]
        public void CreateTokenWithNullCredentials()
        {
            var response = _sut.CreateToken(null, null, "password");
            var code = response.StatusCode.ToString();

            Assert.That(code, Is.EqualTo("OK"));
        }

        [Test]
        public void CreateTokenWithUnauthorizedGrantType()
        {
            var response = _sut.CreateToken("test", "test", "credentials");
            var code = response.StatusCode.ToString();

            Assert.That(code, Is.EqualTo("BadRequest"));
        }

        [Test]
        public void CreateTokenWithAlphaNumericCredentials()
        {
            var response = _sut.CreateToken("asd123", "asd123", "password");
            var code = response.StatusCode.ToString();

            Assert.That(code, Is.EqualTo("OK"));
        }

        [Test]
        public void CreateTokenWithSpecialSymbolCredentials()
        {
            var response = _sut.CreateToken("!@#", "$%^", "password");
            var code = response.StatusCode.ToString();

            Assert.That(code, Is.EqualTo("OK"));
        }

        [Test]
        public void AccessWithInvalidTokenIsNotAllowed()
        {
            //TODO
        }
    }
}
