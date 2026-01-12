using CampusLeague.Domain.Entities;
using Microsoft.AspNetCore.Authentication;

namespace CampusLeague.Tests.TestInfrastructure.Authentication;

public class MockAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public UserRole AuthRole { get; set; }
}
