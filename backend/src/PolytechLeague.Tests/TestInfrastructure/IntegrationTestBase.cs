using Xunit;

namespace PolytechLeague.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase 
        : IClassFixture<PolytechLeagueWebApplicationFactory>
    {
        protected HttpClient httpClient;
        protected readonly PolytechLeagueWebApplicationFactory Factory;

        protected IntegrationTestBase(PolytechLeagueWebApplicationFactory factory)
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
