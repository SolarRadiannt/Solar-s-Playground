namespace SolFramework;

using System.Numerics;

public enum InspectColor
{
    // Whites & Neutrals
    White,
    Gray,
    DarkGray,
    Charcoal,

    // Reds, Pinks & Oranges
    Red,
    Crimson,
    Rose,
    Pink,
    Peach,
    Coral,
    Orange,
    Amber,
    Gold,

    // Yellows & Greens
    Yellow,
    Lime,
    Green,
    Forest,
    Mint,

    // Cyans & Blues
    Teal,
    Cyan,
    SkyBlue,
    Blue,
    Indigo,
    SlateBlue,

    // Purples & Violets
    Lavender,
    Purple,
    Violet,
    Magenta,

    // Earth Tones
    Brown,
    Sand
}

public static class InspectColors
{
    // --- Neutrals ---
    public static readonly Vector4 White     = new Vector4(0.95f, 0.95f, 0.95f, 1.0f);
    public static readonly Vector4 Gray      = new Vector4(0.55f, 0.55f, 0.60f, 1.0f);
    public static readonly Vector4 DarkGray  = new Vector4(0.35f, 0.35f, 0.38f, 1.0f);
    public static readonly Vector4 Charcoal  = new Vector4(0.22f, 0.22f, 0.25f, 1.0f);

    // --- Reds, Pinks & Warm Tones ---
    public static readonly Vector4 Red       = new Vector4(0.88f, 0.28f, 0.28f, 1.0f);
    public static readonly Vector4 Crimson   = new Vector4(0.70f, 0.15f, 0.22f, 1.0f);
    public static readonly Vector4 Rose      = new Vector4(0.92f, 0.40f, 0.55f, 1.0f);
    public static readonly Vector4 Pink      = new Vector4(0.95f, 0.50f, 0.75f, 1.0f);
    public static readonly Vector4 Peach     = new Vector4(0.95f, 0.60f, 0.50f, 1.0f);
    public static readonly Vector4 Coral     = new Vector4(0.92f, 0.45f, 0.35f, 1.0f);
    public static readonly Vector4 Orange    = new Vector4(0.92f, 0.52f, 0.18f, 1.0f);
    public static readonly Vector4 Amber     = new Vector4(0.95f, 0.65f, 0.15f, 1.0f);
    public static readonly Vector4 Gold      = new Vector4(0.90f, 0.75f, 0.18f, 1.0f);

    // --- Yellows & Greens ---
    public static readonly Vector4 Yellow    = new Vector4(0.88f, 0.82f, 0.22f, 1.0f);
    public static readonly Vector4 Lime      = new Vector4(0.62f, 0.85f, 0.20f, 1.0f);
    public static readonly Vector4 Green     = new Vector4(0.35f, 0.78f, 0.35f, 1.0f);
    public static readonly Vector4 Forest    = new Vector4(0.22f, 0.52f, 0.30f, 1.0f);
    public static readonly Vector4 Mint      = new Vector4(0.30f, 0.85f, 0.62f, 1.0f);

    // --- Cyans & Blues ---
    public static readonly Vector4 Teal      = new Vector4(0.20f, 0.80f, 0.78f, 1.0f);
    public static readonly Vector4 Cyan      = new Vector4(0.18f, 0.82f, 0.92f, 1.0f);
    public static readonly Vector4 SkyBlue   = new Vector4(0.38f, 0.72f, 0.98f, 1.0f);
    public static readonly Vector4 Blue      = new Vector4(0.28f, 0.55f, 0.90f, 1.0f);
    public static readonly Vector4 Indigo    = new Vector4(0.38f, 0.38f, 0.88f, 1.0f);
    public static readonly Vector4 SlateBlue = new Vector4(0.45f, 0.52f, 0.68f, 1.0f);

    // --- Purples & Violets ---
    public static readonly Vector4 Lavender  = new Vector4(0.72f, 0.62f, 0.92f, 1.0f);
    public static readonly Vector4 Purple    = new Vector4(0.62f, 0.32f, 0.88f, 1.0f);
    public static readonly Vector4 Violet    = new Vector4(0.48f, 0.22f, 0.78f, 1.0f);
    public static readonly Vector4 Magenta   = new Vector4(0.85f, 0.25f, 0.75f, 1.0f);

    // --- Earth Tones ---
    public static readonly Vector4 Brown     = new Vector4(0.58f, 0.38f, 0.22f, 1.0f);
    public static readonly Vector4 Sand      = new Vector4(0.82f, 0.72f, 0.58f, 1.0f);

    public static Vector4 Get(InspectColor color) => color switch
    {
        // Neutrals
        InspectColor.White     => White,
        InspectColor.Gray      => Gray,
        InspectColor.DarkGray  => DarkGray,
        InspectColor.Charcoal  => Charcoal,

        // Warm Tones
        InspectColor.Red       => Red,
        InspectColor.Crimson   => Crimson,
        InspectColor.Rose      => Rose,
        InspectColor.Pink      => Pink,
        InspectColor.Peach     => Peach,
        InspectColor.Coral     => Coral,
        InspectColor.Orange    => Orange,
        InspectColor.Amber     => Amber,
        InspectColor.Gold      => Gold,

        // Yellows & Greens
        InspectColor.Yellow    => Yellow,
        InspectColor.Lime      => Lime,
        InspectColor.Green     => Green,
        InspectColor.Forest    => Forest,
        InspectColor.Mint      => Mint,

        // Cyans & Blues
        InspectColor.Teal      => Teal,
        InspectColor.Cyan      => Cyan,
        InspectColor.SkyBlue   => SkyBlue,
        InspectColor.Blue      => Blue,
        InspectColor.Indigo    => Indigo,
        InspectColor.SlateBlue => SlateBlue,

        // Purples
        InspectColor.Lavender  => Lavender,
        InspectColor.Purple    => Purple,
        InspectColor.Violet    => Violet,
        InspectColor.Magenta   => Magenta,

        // Earth Tones
        InspectColor.Brown     => Brown,
        InspectColor.Sand      => Sand,

        _                      => White
    };
}