namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class TestCharmRuleTests
{
    private static LamaInfo Loose() =>
        new() { Id = 1, IsOwner = true, InPen = false, Charmed = false, Despawning = false, State = "Idle" };

    [Fact]
    public void Uncharmed_lama_outside_the_pen_on_the_host_is_charmed() => Assert.True(TestCharmRule.ShouldCharm(Loose()));

    [Fact]
    public void Not_on_the_host_is_left_alone()
    {
        var lama = Loose();
        lama.IsOwner = false;
        Assert.False(TestCharmRule.ShouldCharm(lama));
    }

    [Fact]
    public void Lama_in_the_pen_is_left_alone()
    {
        var lama = Loose();
        lama.InPen = true;
        Assert.False(TestCharmRule.ShouldCharm(lama));
    }

    [Fact]
    public void Already_charmed_lama_is_left_alone()
    {
        var lama = Loose();
        lama.Charmed = true;
        Assert.False(TestCharmRule.ShouldCharm(lama));
    }

    [Fact]
    public void Despawning_lama_is_left_alone()
    {
        var lama = Loose();
        lama.Despawning = true;
        Assert.False(TestCharmRule.ShouldCharm(lama));
    }
}
