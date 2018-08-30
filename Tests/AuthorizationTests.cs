using System;
using System.Globalization;
using System.Net;
using API_Assessment.Helpers;
using API_Assessment.Models;
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

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK), "Status code after token creation was not 'Ok'.");
        }

        [Test]
        public void IssuedAndExpiresPropertiesAreAsExpectedInResponseBody()
        {
            var parsed = CreateTokenParseTokenResponseAndSetActualDateTime(out var nowTime);
            var issuedToDateTime = DateTime.ParseExact(parsed.Issued, DateFormat, CultureInfo.InvariantCulture);

            Assert.That(issuedToDateTime, Is.EqualTo(nowTime).Within(2).Seconds, "'.issued' property differs from expected.");
        }

        [Test]
        public void ExpiresPropertyIsAsExpectedInResponseBody()
        {
            var parsed = CreateTokenParseTokenResponseAndSetActualDateTime(out var nowTime);
            var expiresToDateTime = DateTime.ParseExact(parsed.Expires, DateFormat, CultureInfo.InvariantCulture);

            Assert.That(expiresToDateTime, Is.EqualTo(nowTime.AddMinutes(5)).Within(2).Seconds, 
                "'.expires' property differs from expected.");
        }

        [Test]
        public void TokenTypePropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).TokenType;

            Assert.That(parsed, Is.EqualTo("bearer"), "'token_type' property differs from expected.");
        }

        [Test]
        public void ExpiresInPropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).ExpiresIn;

            Assert.That(parsed, Is.EqualTo("299"), "'expires_in' property differs from expected.");
        }

        [Test]
        public void DisplayNamePropertyIsAsExpectedInResponseBody()
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response).DisplayName;

            Assert.That(parsed, Is.EqualTo("testName_alias"), "'displayName' property differs from expected.");
        }

        [Test]
        public void CreateTokenWithEmptyCredentials()
        {
            var code = CreateTokenAndGetStatusCode("", "", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK), "Creating token with empty credentials is not allowed, but should be.");
        }

        [Test]
        public void CreateTokenWithNullCredentials()
        {
            var code = CreateTokenAndGetStatusCode(null, null, "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK), "Creating token with null credentials is not allowed, but should be.");
        }

        [Test]
        public void CreateTokenWithUnauthorizedGrantType()
        {
            var code = CreateTokenAndGetStatusCode("test", "test", "credentials");

            Assert.That(code, Is.EqualTo(HttpStatusCode.BadRequest), 
                "Creating token with the wrong grant type is  allowed, but should not be.");
        }

        [Test]
        public void CreateTokenWithAlphaNumericCredentials()
        {
            var code = CreateTokenAndGetStatusCode("asd123", "asd123", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK), 
                "Creating token with alphanumeric credentials is not allowed, but should be.");
        }

        [Test]
        public void CreateTokenWithSpecialSymbolCredentials()
        {
            var code = CreateTokenAndGetStatusCode("!@#", "$%^", "password");

            Assert.That(code, Is.EqualTo(HttpStatusCode.OK), 
                "Creating token with special symbol credentials is not allowed, but should be.");
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

        /// <summary>
        /// Creates token, parses JSON response and sets actual DateTime.
        /// </summary>
        /// <param name="nowTime">Actual time at the moment of assigment; is passed by reference</param>
        /// <returns>Parsed TokenModel values</returns>
        private TokenModel CreateTokenParseTokenResponseAndSetActualDateTime(out DateTime nowTime)
        {
            var response = _sut.CreateToken();
            var parsed = _sut.ParseTokenResponse(response);
            nowTime = DateTime.Now;
            return parsed;
        }
    }
}
