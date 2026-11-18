namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class StateNamesTests
{
    [Theory]
    [InlineData("HerdingTargetFollowState", "Follow")]
    [InlineData("HerdingTargetIdleState", "Idle")]
    [InlineData("HerdingTargetWalkingState", "Walking")]
    [InlineData("HerdingTargetRunningState", "Running")]
    public void Game_state_types_get_their_short_name(string typeName, string expected) =>
        Assert.Equal(expected, StateNames.Short(typeName));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void No_state_is_none(string? typeName) => Assert.Equal("none", StateNames.Short(typeName));

    [Theory]
    [InlineData("SomethingElse")]
    [InlineData("HerdingTargetState")]
    public void Unknown_names_are_kept_as_they_are(string typeName) => Assert.Equal(typeName, StateNames.Short(typeName));

    [Fact]
    public void Follow_constant_matches_the_follow_state() =>
        Assert.Equal(StateNames.Follow, StateNames.Short("HerdingTargetFollowState"));
}
