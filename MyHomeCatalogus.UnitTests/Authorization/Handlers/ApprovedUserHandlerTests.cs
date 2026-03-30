using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyHomeCatalogus.Authorization.Handlers;
using MyHomeCatalogus.Authorization.Requirements;
using MyHomeCatalogus.Data;
using System.Security.Claims;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Authorization.Handlers
{
    public class ApprovedUserHandlerTests
    {
        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());
        }

        private class TestableApprovedUserHandler : ApprovedUserHandler
        {
            public TestableApprovedUserHandler(UserManager<ApplicationUser> userManager) : base(userManager) { }

            public async Task TestHandleRequirementAsync(AuthorizationHandlerContext context, ApprovedUserRequirement requirement)
            {
                await HandleRequirementAsync(context, requirement);
            }
        }

        [Fact]
        public async Task HandleRequirementAsync_Succeeds_WhenUserIsApproved()
        {
            var userManager = CreateUserManagerMock();
            var user = new ApplicationUser { Id = "user1", IsApproved = true };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user1") }));

            userManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

            var handler = new TestableApprovedUserHandler(userManager.Object);
            var requirement = new ApprovedUserRequirement();
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrincipal, null);

            await handler.TestHandleRequirementAsync(context, requirement);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fails_WhenUserIsNotApproved()
        {
            var userManager = CreateUserManagerMock();
            var user = new ApplicationUser { Id = "user1", IsApproved = false };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user1") }));

            userManager.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);

            var handler = new TestableApprovedUserHandler(userManager.Object);
            var requirement = new ApprovedUserRequirement();
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrincipal, null);

            await handler.TestHandleRequirementAsync(context, requirement);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fails_WhenUserIsNull()
        {
            var userManager = CreateUserManagerMock();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            userManager.Setup(x => x.GetUserAsync(claimsPrincipal)).Returns(Task.FromResult<ApplicationUser?>(null));

            var handler = new TestableApprovedUserHandler(userManager.Object);
            var requirement = new ApprovedUserRequirement();
            var context = new AuthorizationHandlerContext(new[] { requirement }, claimsPrincipal, null);

            await handler.TestHandleRequirementAsync(context, requirement);

            Assert.False(context.HasSucceeded);
        }
    }
}