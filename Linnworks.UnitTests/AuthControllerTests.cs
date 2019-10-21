using LinnworksTest.Controllers;
using LinnworksTest.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static LinnworksTest.Controllers.AuthController;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Linq;

namespace Linnworks.UnitTests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        static Account[] InvalidAccountCases =
        {
            null,
            new Account(){ Token=string.Empty },
            new Account(){ Token="NOT_A_GUID" }
        };
        private Mock<ITokenRepository> _mockRepository;
        private Mock<IAuthenticationService> _authServiceMock;
        private AuthController _sut;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITokenRepository>();
            _authServiceMock = new Mock<IAuthenticationService>();
            _authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(_authServiceMock.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    RequestServices = serviceProviderMock.Object
                }
            };
            _sut = new AuthController(_mockRepository.Object) { ControllerContext = controllerContext };
        }

        [Test, TestCaseSource("InvalidAccountCases")]
        public async Task Login_ShouldReturnBadRequest_WhenAccount_IsInvalid(Account invalidAccount)
        {
            // .Act
            var result = await _sut.Login(invalidAccount);

            // .Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test, TestCaseSource("InvalidAccountCases")]
        public async Task Login_ShouldNotAuthenticate_WhenAccount_IsInvalid(Account invalidAccount)
        {
            // .Act
            var result = await _sut.Login(invalidAccount);

            // .Assert
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Never);
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenAccount_DoesNotExist()
        {
            // .Arrange
            _mockRepository.Setup(r => r.IsValidTokenAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));
            var notExistingAccount = new Account() { Token = Guid.NewGuid().ToString() };

            // .Act
            var result = await _sut.Login(notExistingAccount);

            // .Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_ShouldNotAuthenticate_WhenAccount_DoesNotExist()
        {
            // .Arrange
            _mockRepository.Setup(r => r.IsValidTokenAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));
            var notExistingAccount = new Account() { Token = Guid.NewGuid().ToString() };

            // .Act
            var result = await _sut.Login(notExistingAccount);

            // .Assert
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Never);
        }

        [Test]
        public async Task Login_ShouldReturnOkResult_WhenAccount_Exists()
        {
            // .Arrange
            _mockRepository.Setup(r => r.IsValidTokenAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            var existingAccount = new Account() { Token = Guid.NewGuid().ToString() };
            // .Act
            var result = await _sut.Login(existingAccount);

            // .Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(existingAccount.Token, (result as OkObjectResult).Value);
        }

        [Test]
        public async Task Login_ShouldAuthenticate_WhenAccount_Exists()
        {
            // .Arrange
            _mockRepository.Setup(r => r.IsValidTokenAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            var existingAccount = new Account() { Token = Guid.NewGuid().ToString() };
            // .Act
            var result = await _sut.Login(existingAccount);

            // .Assert
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.Is<ClaimsPrincipal>(cp => cp.Claims.Any(c => c.Value == existingAccount.Token)),
                It.IsAny<AuthenticationProperties>()), Times.Once);
        }

    }
}
