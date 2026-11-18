namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class WalkWatchdogTests
{
    private const float Max = WalkWatchdog.MaxWalkSeconds;

    [Fact]
    public void Unknown_lama_is_never_stopped() => Assert.False(new WalkWatchdog().ShouldStop(1, StateNames.Walking, 100f));

    [Fact]
    public void Still_walking_within_the_limit_is_left_alone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.False(w.ShouldStop(1, StateNames.Walking, 10f + Max - 0.1f));
    }

    [Fact]
    public void Still_walking_after_the_limit_is_stopped_once()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.True(w.ShouldStop(1, StateNames.Walking, 10f + Max));
        Assert.False(w.ShouldStop(1, StateNames.Walking, 10f + Max + 1f));
    }

    [Fact]
    public void Arrived_lama_is_forgotten_so_its_later_wandering_is_left_alone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.False(w.ShouldStop(1, "Idle", 12f));
        Assert.False(w.ShouldStop(1, StateNames.Walking, 10f + Max + 5f)); // the game's own wander walk
    }

    [Fact]
    public void Lamas_are_watched_separately()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        w.Started(2, 15f);
        Assert.True(w.ShouldStop(1, StateNames.Walking, 10f + Max));
        Assert.False(w.ShouldStop(2, StateNames.Walking, 10f + Max));
    }

    [Fact]
    public void Clear_forgets_everyone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        w.Clear();
        Assert.False(w.ShouldStop(1, StateNames.Walking, 10f + Max));
    }
}
