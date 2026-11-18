namespace SpellBrigadeLamaPen.Tests;

using System;
using SpellBrigadeLamaPen.Core;
using Xunit;

public class PenSpotTests
{
    private const float Radius = 10f;
    private static readonly FlatPoint Center = new(100f, 200f);

    private static float DistanceFromCenter(FlatPoint p) =>
        MathF.Sqrt((p.X - Center.X) * (p.X - Center.X) + (p.Z - Center.Z) * (p.Z - Center.Z));

    [Fact]
    public void Middle_random_values_go_straight_in_on_the_side_the_lama_came_from()
    {
        var lama = new FlatPoint(Center.X + Radius, Center.Z); // east edge
        var spot = PenSpot.Pick(lama, Center, Radius, 0.5f, 0.5f);
        Assert.Equal(Center.X + Radius * 0.30f, spot.X, 3); // halfway between 15% and 45%
        Assert.Equal(Center.Z, spot.Z, 3);
    }

    [Fact]
    public void Lowest_distance_is_15_percent_of_the_radius()
    {
        var spot = PenSpot.Pick(new FlatPoint(Center.X, Center.Z - Radius), Center, Radius, 0f, 0.5f);
        Assert.Equal(Radius * PenSpot.MinFraction, DistanceFromCenter(spot), 3);
    }

    [Fact]
    public void Highest_distance_is_45_percent_of_the_radius()
    {
        var spot = PenSpot.Pick(new FlatPoint(Center.X, Center.Z - Radius), Center, Radius, 1f, 0.5f);
        Assert.Equal(Radius * PenSpot.MaxFraction, DistanceFromCenter(spot), 3);
    }

    [Theory]
    [InlineData(0f, -60f)]
    [InlineData(1f, 60f)]
    [InlineData(0.75f, 30f)]
    public void Turn_is_at_most_60_degrees_either_way(float turnRandom, float expectedDegrees)
    {
        var lama = new FlatPoint(Center.X + Radius, Center.Z); // direction 0 degrees
        var spot = PenSpot.Pick(lama, Center, Radius, 0.5f, turnRandom);
        var degrees = MathF.Atan2(spot.Z - Center.Z, spot.X - Center.X) * 180f / MathF.PI;
        Assert.Equal(expectedDegrees, degrees, 2);
    }

    [Fact]
    public void Lama_exactly_at_the_center_still_gets_a_spot_inside_the_inner_ring()
    {
        var spot = PenSpot.Pick(Center, Center, Radius, 0.5f, 0.5f);
        var d = DistanceFromCenter(spot);
        Assert.InRange(d, Radius * PenSpot.MinFraction - 0.001f, Radius * PenSpot.MaxFraction + 0.001f);
    }

    [Fact]
    public void Lama_outside_the_pen_is_still_sent_to_the_inner_ring()
    {
        var spot = PenSpot.Pick(new FlatPoint(Center.X - 3 * Radius, Center.Z), Center, Radius, 1f, 0.5f);
        Assert.Equal(Radius * PenSpot.MaxFraction, DistanceFromCenter(spot), 3);
        Assert.True(spot.X < Center.X); // same side it came from
    }
}
