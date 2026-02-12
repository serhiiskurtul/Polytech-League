using PolytechLeague.Domain.Entities;
using Microsoft.AspNetCore.Authentication;

namespace PolytechLeague.Tests.TestInfrastructure.Authentication;

public class MockAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public UserRole AuthRole { get; set; }
}
