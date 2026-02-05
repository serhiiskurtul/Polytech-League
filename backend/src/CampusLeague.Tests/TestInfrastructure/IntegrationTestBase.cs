using Xunit;

namespace CampusLeague.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase 
        : IClassFixture<CampusLeagueWebApplicationFactory>
    {
        protected HttpClient httpClient;
        protected readonly CampusLeagueWebApplicationFactory Factory;

        protected IntegrationTestBase(CampusLeagueWebApplicationFactory factory)
        {
            Factory = factory;
            httpClient = factory.CreateClient();
        }

        protected void RecreateClient()
        {
            httpClient = Factory.CreateClient();
        }
    }
}
