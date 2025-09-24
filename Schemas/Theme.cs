public class Theme
{
    public Colour Background { get; set; } = new Colour { R = 23, G = 36, B = 42 };
    public Colour WindowHighlight { get; set; } = new Colour { R = 255, G = 255, B = 255 };
    public Colour WindowBase { get; set; } = new Colour { R = 210, G = 210, B = 210 };
    public Colour Dialog { get; set; } = new Colour { R = 195, G = 232, B = 141 };
    public Colour MenuBarText { get; set; } = new Colour { R = 255, G = 255, B = 255 };
    public Colour MenuBarBackground { get; set; } = new Colour { R = 23, G = 36, B = 42 };
}

public class Colour
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}
