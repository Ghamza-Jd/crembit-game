using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the Color the Avatar's tint, hair, shirt, and pants.
/// </summary>
public static class Style
{
    public static readonly Dictionary<string, Color> TintColors = new Dictionary<string, Color>
    {
        {"white",  new Color(1.0f, 1.0f, 1.0f)},
        {"brown", new Color(0.66f, 0.49f, 0.27f)},
        {"black",  new Color(0.37f, 0.32f, 0.32f)},
        {"green",  Color.green}
    };

    public static readonly Dictionary<string, Color> HairColors = new Dictionary<string, Color>
    {
        {"white", new Color(1f, 1f, 1f)},
        {"black", new Color(0f, 0f, 0f)},
        {"brown", new Color(0.44f, 0.29f, 0.09f)},
        {"blond", new Color(1f, 0.82f, 0f)}
    };
    
    public static readonly Dictionary<string, Color> DefaultColors = new Dictionary<string, Color>
    {
        {"white", Color.white},
        {"black", Color.black},
        {"red", Color.red},
        {"green", Color.green},
        {"grey", Color.grey},
        {"blue", Color.blue},
        {"yellow", Color.yellow},
        {"magenta", Color.magenta},
        {"cyan", Color.cyan}
    };

    public static readonly Dictionary<string, Color> StyleColors = new Dictionary<string, Color>
    {
        {"shirt", DefaultColors["red"]},
        {"pants", DefaultColors["blue"]},
        {"hair", HairColors["brown"]},
        {"tint", TintColors["white"]}
    };
}
