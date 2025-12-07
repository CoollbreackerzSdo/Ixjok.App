namespace Ixjok.Resources.Styles;

public sealed partial class Colors : ResourceDictionary
{
    public Colors()
    {
        InitializeComponent();
    }
    public static Color PeachFuzz { get; } = Color.FromArgb("#FFCDB2FF");
    public static Color PowderBlush { get; } = Color.FromArgb("#FFB4A2FF");
    public static Color CottonCandy { get; } = Color.FromArgb("#FFE5989B");
    public static Color DustyRose { get; } = Color.FromArgb("#FFB5838D");
    public static Color DimGrey { get; } = Color.FromArgb("#FF1E1B22");
}