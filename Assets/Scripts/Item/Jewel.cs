using UnityEngine;
using System.Collections;
using System;

public class Jewel : Valuable
{
    public enum Type
    {
        Aquamarine,
        Emerald,
        GoldenStar,
        MoonStone,
        Obsidian,
        PinkSaphire,
        Ruby
    }

    public Type type;
}

