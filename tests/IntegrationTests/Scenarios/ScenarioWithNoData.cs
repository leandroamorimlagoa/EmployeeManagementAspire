using IntegrationTests.Core;

namespace IntegrationTests.Scenarios;

public class ScenarioWithNoData : BaseIntegrationTest
{
    public ScenarioWithNoData(IntegrationTestFactory factory) : base(factory)
    {
        _database.ApplicationUser.RemoveRange(_database.ApplicationUser);
        _database.SaveChanges();
    }
}
