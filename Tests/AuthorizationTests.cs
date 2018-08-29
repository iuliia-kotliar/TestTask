using System;
using System.Globalization;
using System.Net;
using RestSharp;
using API_Assessment.Helpers;
using NUnit.Framework;

namespace API_Assessment.Tests
{
    [TestFixture]
    public class AuthorizationTests
    {
        private RESTHelper _sut;
        private const string DateFormat = "ddd, dd MMM yyyy HH:mm:ss GMT";

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

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void IssuedAndExpiresPropertiesAreAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response);
            var nowTime = DateTime.Now;
            var issuedToDateTime = DateTime.ParseExact(parsed.Issued, DateFormat, CultureInfo.InvariantCulture);
            var iexpiresToDateTime = DateTime.ParseExact(parsed.Expires, DateFormat, CultureInfo.InvariantCulture);
            Assert.That(issuedToDateTime, Is.EqualTo(nowTime).Within(2).Seconds);
            Assert.That(iexpiresToDateTime, Is.EqualTo(nowTime.AddMinutes(5)).Within(2).Seconds);
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
            var code = CreateTokenAndGetStatusCode("", "", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CreateTokenWithNullCredentials()
        {
            var code = CreateTokenAndGetStatusCode(null, null, "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CreateTokenWithUnauthorizedGrantType()
        {
            var code = CreateTokenAndGetStatusCode("test", "test", "credentials");

            Assert.That(code, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void CreateTokenWithAlphaNumericCredentials()
        {
            var code = CreateTokenAndGetStatusCode("asd123", "asd123", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CreateTokenWithSpecialSymbolCredentials()
        {
            var code = CreateTokenAndGetStatusCode("!@#", "$%^", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
        }

        /// <summary>
        /// Creates token and gets HTTP status code of the token creation request.
        /// </summary>
        /// <param name="username">username value</param>
        /// <param name="password">password value</param>
        /// <param name="grantType">gran_type value</param>
        /// <returns></returns>
        private HttpStatusCode CreateTokenAndGetStatusCode(string username, string password, string grantType)
        {
            var response = _sut.CreateToken(username, password, grantType);
            var code = response.StatusCode;
            return code;
        }
    }
}
