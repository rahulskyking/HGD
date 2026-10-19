using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Tests.Enums;

public class UserRoleExtensionsTests
{
    [Theory]
    [InlineData(UserRole.Candidate, true)]
    [InlineData(UserRole.Studio, true)]
    [InlineData(UserRole.Admin, false)]
    public void Only_candidates_and_studios_can_self_register(UserRole role, bool expected)
    {
        Assert.Equal(expected, role.CanSelfRegister());
    }
}
