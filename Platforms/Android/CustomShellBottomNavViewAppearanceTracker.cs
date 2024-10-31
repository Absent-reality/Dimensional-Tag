using Android.Graphics.Drawables;
using Android.Views;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace DimensionalTag;

//This for the TabBar item customization.
internal class CustomShellBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
	: ShellBottomNavViewAppearanceTracker(shellContext, shellItem)
{
	private readonly IShellContext shellContext = shellContext;

	public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
	{
		base.SetAppearance(bottomView, appearance);
		if (Shell.GetTabBarIsVisible(shellContext.Shell.CurrentPage))
		{
			var backgroundDrawable = new GradientDrawable();
			backgroundDrawable.SetShape(ShapeType.Rectangle);
			// backgroundDrawable.SetCornerRadius(30);
			backgroundDrawable.SetColor(appearance.EffectiveTabBarBackgroundColor.ToPlatform());
			bottomView.SetBackground(backgroundDrawable);

			var layoutParams = bottomView.LayoutParameters;
			if (layoutParams is ViewGroup.MarginLayoutParams marginLayoutParams)
			{
				//Set screen margins for TabBar
				const int margin = 10;
				marginLayoutParams.BottomMargin = margin;
				marginLayoutParams.LeftMargin = margin;
				marginLayoutParams.RightMargin = margin;
				bottomView.LayoutParameters = layoutParams;
			}
		}
	}

	protected override void SetBackgroundColor(BottomNavigationView bottomView, Microsoft.Maui.Graphics.Color color)
	{
		base.SetBackgroundColor(bottomView, color);
		bottomView.RootView?.SetBackgroundColor(shellContext.Shell.CurrentPage.BackgroundColor.ToPlatform());
	}
}