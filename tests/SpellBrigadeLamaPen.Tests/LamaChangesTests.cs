namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class LamaChangesTests
{
    private static LamaInfo Lama(int id, string state, bool inPen, bool charmed) =>
        new() { Id = id, IsOwner = true, InPen = inPen, Charmed = charmed, State = state };

    [Fact]
    public void First_sighting_is_logged_as_new()
    {
        var changes = new LamaChanges();
        Assert.Equal("lama 7: new, Idle, outside, charmed", changes.Describe(Lama(7, "Idle", false, true)));
    }

    [Fact]
    public void Nothing_changed_gives_null()
    {
        var changes = new LamaChanges();
        changes.Describe(Lama(7, "Idle", false, true));
        Assert.Null(changes.Describe(Lama(7, "Idle", false, true)));
    }

    [Fact]
    public void A_change_shows_before_and_after()
    {
        var changes = new LamaChanges();
        changes.Describe(Lama(7, "Idle", false, true));
        Assert.Equal("lama 7: Idle, outside, charmed -> Follow, in pen, charmed",
            changes.Describe(Lama(7, "Follow", true, true)));
    }

    [Fact]
    public void Lamas_are_tracked_separately()
    {
        var changes = new LamaChanges();
        changes.Describe(Lama(7, "Idle", false, true));
        Assert.Equal("lama 8: new, Idle, outside, charmed", changes.Describe(Lama(8, "Idle", false, true)));
        Assert.Null(changes.Describe(Lama(7, "Idle", false, true)));
    }

    [Fact]
    public void After_clear_every_lama_is_new_again()
    {
        var changes = new LamaChanges();
        changes.Describe(Lama(7, "Idle", false, true));
        changes.Clear();
        Assert.Equal("lama 7: new, Idle, outside, charmed", changes.Describe(Lama(7, "Idle", false, true)));
    }
}
