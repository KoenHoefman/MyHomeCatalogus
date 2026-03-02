using MyHomeCatalogus.Components;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Components.Pages.Shared
{
	public class RouteConstantsTests
	{
		private const string TestBaseRoute = "/testentity";
		private const int TestId = 42;

		[Fact]
		public void GetAddRoute_ShouldAppendAddSuffixToBaseRoute()
		{
			var expectedRoute = $"{TestBaseRoute}{RouteConstants.AddSuffix}";

			var actualRoute = RouteConstants.GetAddRoute(TestBaseRoute);

			Assert.Equal(expectedRoute, actualRoute);
		}

		[Fact]
		public void GetDetailRoute_ShouldCorrectlySubstituteIdInDetailSuffix()
		{
			var expectedRoute = "/testentity/42";

			var actualRoute = RouteConstants.GetDetailRoute(TestBaseRoute, TestId);

			Assert.Equal(expectedRoute, actualRoute);
		}

		[Fact]
		public void GetEditRoute_ShouldCorrectlySubstituteIdInEditSuffix()
		{
			var expectedRoute = "/testentity/42/edit";

			var actualRoute = RouteConstants.GetEditRoute(TestBaseRoute, TestId);

			Assert.Equal(expectedRoute, actualRoute);
		}

		[Fact]
		public void GetDeleteRoute_ShouldCorrectlySubstituteIdInDeleteSuffix()
		{
			var expectedRoute = "/testentity/42/delete";

			var actualRoute = RouteConstants.GetDeleteRoute(TestBaseRoute, TestId);

			Assert.Equal(expectedRoute, actualRoute);
		}

		[Fact]
		public void GetRouteWithIdParameter_ShouldReplacePlaceholderWithProvidedId()
		{
			const string routeTemplate = "/api/v1/resource/{id:int}";
			const int id = 99;
			const string expectedRoute = "/api/v1/resource/99";

			var actualRoute = RouteConstants.GetRouteWithIdParameter(routeTemplate, id);

			Assert.Equal(expectedRoute, actualRoute);
		}

		[Fact]
		public void GetRouteWithIdParameter_ShouldHandleNoPlaceholderGracefully()
		{
			const string routeTemplate = "/api/v1/resource";
			const int id = 10;
			const string expectedRoute = "/api/v1/resource";

			var actualRoute = RouteConstants.GetRouteWithIdParameter(routeTemplate, id);

			Assert.Equal(expectedRoute, actualRoute);
		}


	}
}
