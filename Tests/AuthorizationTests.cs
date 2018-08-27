using System.Threading;
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
        public void AccessWithoutTokenIsNotAllowed()
        {
            _sut = new RESTHelper(false);
            var response = _sut.CreateToken();
            var code = response.StatusCode.ToString();
            Assert.That(code, Is.EqualTo("OK"));
        }
    }
}
