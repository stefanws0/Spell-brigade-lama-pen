namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class WalkWatchdogTests
{
    private const float Max = WalkWatchdog.MaxWalkSeconds;

    private static LamaInfo Lama(int id, string state) =>
        new() { Id = id, IsOwner = true, InPen = true, Charmed = false, Despawning = false, State = state };

    private static LamaInfo Walking(int id) => Lama(id, StateNames.Walking);

    [Fact]
    public void Unknown_lama_is_never_stopped() => Assert.False(new WalkWatchdog().ShouldStop(Walking(1), 100f));

    [Fact]
    public void Still_walking_within_the_limit_is_left_alone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.False(w.ShouldStop(Walking(1), 10f + Max - 0.01f));
    }

    [Fact]
    public void Still_walking_after_the_limit_is_stopped_once()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.True(w.ShouldStop(Walking(1), 10f + Max));
        Assert.False(w.ShouldStop(Walking(1), 10f + Max + 1f));
    }

    [Fact]
    public void Just_under_the_limit_then_at_the_limit()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.False(w.ShouldStop(Walking(1), 10f + Max - 0.01f));
        Assert.True(w.ShouldStop(Walking(1), 10f + Max));
    }

    [Fact]
    public void Starting_again_restarts_the_timer()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        w.Started(1, 15f);
        Assert.False(w.ShouldStop(Walking(1), 10f + Max));
        Assert.True(w.ShouldStop(Walking(1), 15f + Max));
    }

    [Fact]
    public void Arrived_lama_is_forgotten_so_its_later_wandering_is_left_alone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        Assert.False(w.ShouldStop(Lama(1, "Idle"), 12f));
        Assert.False(w.ShouldStop(Walking(1), 10f + Max + 5f)); // the game's own wander walk
    }

    [Fact]
    public void Despawning_lama_is_never_stopped_and_is_forgotten()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        var despawning = Walking(1);
        despawning.Despawning = true;
        Assert.False(w.ShouldStop(despawning, 10f + Max));
        Assert.False(w.ShouldStop(Walking(1), 10f + Max + 1f)); // forgotten, even if seen walking again
    }

    [Fact]
    public void Lama_this_pc_does_not_control_is_never_stopped_and_is_forgotten()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        var notOurs = Walking(1);
        notOurs.IsOwner = false;
        Assert.False(w.ShouldStop(notOurs, 10f + Max));
        Assert.False(w.ShouldStop(Walking(1), 10f + Max + 1f));
    }

    [Fact]
    public void Lamas_are_watched_separately()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        w.Started(2, 15f);
        Assert.True(w.ShouldStop(Walking(1), 10f + Max));
        Assert.False(w.ShouldStop(Walking(2), 10f + Max));
    }

    [Fact]
    public void Clear_forgets_everyone()
    {
        var w = new WalkWatchdog();
        w.Started(1, 10f);
        w.Clear();
        Assert.False(w.ShouldStop(Walking(1), 10f + Max));
    }
}
