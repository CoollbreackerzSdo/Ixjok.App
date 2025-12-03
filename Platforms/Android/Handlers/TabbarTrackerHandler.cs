using Android.Graphics;
using Android.Views;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using AndroidD = Android.Graphics.Drawables;
namespace Ixjok;

public sealed class ShellTrackerRenderer : ShellRenderer
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        return new TabbarTrackerHandler(this, shellItem);
    }

    protected override IShellTabLayoutAppearanceTracker CreateTabLayoutAppearanceTracker(ShellSection shellSection)
    {
        return new TabLayoutTrackerHandler(this);
    }
}
public sealed class TabLayoutTrackerHandler(IShellContext shellContext) : ShellTabLayoutAppearanceTracker(shellContext)
{
    public override void SetAppearance(TabLayout tabLayout, ShellAppearance appearance)
    {
        base.SetAppearance(tabLayout, appearance);
    }
}
public sealed class TabbarTrackerHandler(IShellContext shellContext, ShellItem shellItem) : ShellBottomNavViewAppearanceTracker(shellContext, shellItem)
{
    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        base.SetAppearance(bottomView, appearance);
        //
        // var background = new AndroidD.GradientDrawable();
        // background.SetColor(appearance.EffectiveTabBarBackgroundColor.ToPlatform());
        // // bottomView.SetBackground(background);
        bottomView.LayoutParameters!.Height = 200;
        bottomView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;
        bottomView.SetForegroundGravity(GravityFlags.CenterVertical | GravityFlags.RelativeHorizontalGravityMask);
    }
}