namespace SpellBrigadeLamaPen.Core;

using System;

/// <summary>A point on the ground (Unity's x and z), so Core needs no Unity types.</summary>
internal struct FlatPoint
{
    public float X;
    public float Z;

    public FlatPoint(float x, float z)
    {
        X = x;
        Z = z;
    }
}

/// <summary>
/// Where a just-decharmed lama walks to: a spot in the inner part of the pen, 15-45% of the pen's radius from the
/// center, on the side the lama came in from (turned at most 60 degrees either way), so the lamas spread out instead
/// of piling onto one point or crossing the whole pen.
/// </summary>
internal static class PenSpot
{
    public const float MinFraction = 0.15f;
    public const float MaxFraction = 0.45f;
    public const float MaxTurnDegrees = 60f;

    /// <param name="lama">Where the lama is now.</param>
    /// <param name="center">The pen's center.</param>
    /// <param name="penRadius">The pen's radius.</param>
    /// <param name="distanceRandom">0..1: 0 = closest to the center, 1 = farthest.</param>
    /// <param name="turnRandom">0..1: 0 = turned 60 degrees one way, 0.5 = straight in, 1 = 60 degrees the other way.</param>
    public static FlatPoint Pick(FlatPoint lama, FlatPoint center, float penRadius, float distanceRandom, float turnRandom)
    {
        var dx = lama.X - center.X;
        var dz = lama.Z - center.Z;
        var length = MathF.Sqrt(dx * dx + dz * dz);
        if (length < 0.001f)
        {
            dx = 1f; // standing on the center: any direction will do
            dz = 0f;
            length = 1f;
        }

        var angle = (Clamp01(turnRandom) * 2f - 1f) * MaxTurnDegrees * MathF.PI / 180f;
        var cos = MathF.Cos(angle);
        var sin = MathF.Sin(angle);
        var ux = dx / length;
        var uz = dz / length;
        var distance = penRadius * (MinFraction + (MaxFraction - MinFraction) * Clamp01(distanceRandom));
        return new FlatPoint(
            center.X + (ux * cos - uz * sin) * distance,
            center.Z + (ux * sin + uz * cos) * distance);
    }

    private static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
}
