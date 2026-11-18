namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class DecharmRuleTests
{
    private static LamaInfo Penned() =>
        new() { Id = 1, IsOwner = true, InPen = true, Charmed = true, Despawning = false, State = "Follow" };

    [Fact]
    public void Charmed_lama_in_the_pen_on_the_host_is_decharmed() =>
        Assert.True(DecharmRule.ShouldDecharm(Penned(), alreadyDecharmed: false));

    [Fact]
    public void Not_on_the_host_is_left_alone()
    {
        var lama = Penned();
        lama.IsOwner = false;
        Assert.False(DecharmRule.ShouldDecharm(lama, false));
    }

    [Fact]
    public void Outside_the_pen_is_left_alone()
    {
        var lama = Penned();
        lama.InPen = false;
        Assert.False(DecharmRule.ShouldDecharm(lama, false));
    }

    [Fact]
    public void Normal_lama_is_left_alone()
    {
        var lama = Penned();
        lama.Charmed = false;
        Assert.False(DecharmRule.ShouldDecharm(lama, false));
    }

    [Fact]
    public void Despawning_lama_is_left_alone()
    {
        var lama = Penned();
        lama.Despawning = true;
        Assert.False(DecharmRule.ShouldDecharm(lama, false));
    }

    [Fact]
    public void Already_decharmed_lama_is_not_done_twice() =>
        Assert.False(DecharmRule.ShouldDecharm(Penned(), alreadyDecharmed: true));

    [Theory]
    [InlineData("Follow")]
    [InlineData("Idle")]
    [InlineData("Walking")]
    [InlineData("Running")]
    public void State_does_not_matter(string state)
    {
        var lama = Penned();
        lama.State = state;
        Assert.True(DecharmRule.ShouldDecharm(lama, false));
    }
}
