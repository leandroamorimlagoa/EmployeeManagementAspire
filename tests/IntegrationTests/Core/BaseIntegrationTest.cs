using Bogus;
using Domain.Employees;
using Infrastructure.Repositories.Core;
using IntegrationTests.Builders.Entities;
using IntegrationTests.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Core;

public class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    protected HttpClient _client;
    private readonly IntegrationTestFactory factory;

    protected ApplicationDbContext _database
    {
        get
        {
            return this.factory.Services.GetRequiredService<ApplicationDbContext>();
        }
    }

    protected UserManager<ApplicationUser> _userManager
    {
        get
        {
            return this.factory.Services.GetRequiredService<UserManager<ApplicationUser>>();
        }
    }

    protected RoleManager<IdentityRole> _roleManager
    {
        get
        {
            return this.factory.Services.GetRequiredService<RoleManager<IdentityRole>>();
        }
    }

    public BaseIntegrationTest(IntegrationTestFactory factory)
    {
        this._client = factory.CreateClient();
        this.factory = factory;
    }

    protected void WithAuthenticatedUser()
    {
        var password = new Faker().Internet.Password(prefix: "!@#abcGTD197", length: 20);
        var userBuilder = new ApplicationUserBuilder()
                                    .WithFullValidApplicationUser()
                                    .WithPassword(password)
                                    .Save(_userManager, _roleManager);
        var user = userBuilder.Build();
        _client.WithAuthenticatedUser(user);
    }
}
