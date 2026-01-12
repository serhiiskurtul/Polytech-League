using Xunit;

namespace CampusLeague.Tests.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<CampusLeagueWebApplicationFactory>
    {
        protected readonly HttpClient httpClient;

        public IntegrationTestBase(CampusLeagueWebApplicationFactory factory)
        {
            httpClient = factory.CreateClient();
        }
    }
}
