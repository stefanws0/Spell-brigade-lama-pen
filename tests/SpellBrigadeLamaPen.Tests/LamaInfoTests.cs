namespace SpellBrigadeLamaPen.Tests;

using SpellBrigadeLamaPen.Core;
using Xunit;

public class LamaInfoTests
{
    [Fact]
    public void Summary_of_a_charmed_lama_in_the_pen_on_the_host()
    {
        var lama = new LamaInfo { Id = 7, IsOwner = true, InPen = true, Charmed = true, State = "Follow" };
        Assert.Equal("Follow, in pen, charmed", lama.Summary());
    }

    [Fact]
    public void Summary_mentions_not_host_and_despawning()
    {
        var lama = new LamaInfo { Id = 7, IsOwner = false, InPen = false, Charmed = false, Despawning = true, State = "Idle" };
        Assert.Equal("Idle, outside, not charmed, not host, despawning", lama.Summary());
    }
}
