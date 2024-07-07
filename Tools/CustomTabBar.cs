using Maui.BindableProperty.Generator.Core;
using System.Windows.Input;

namespace DimensionalTag
{
    public partial class CustomTabBar : TabBar
    {
        [AutoBindable]
        private ICommand? centerViewCommand;

        [AutoBindable]
        private ImageSource? centerViewImageSource;

        [AutoBindable]
        private string? centerViewText;

        [AutoBindable]
        private bool? centerViewVisible;

        [AutoBindable]
        public Color? centerViewBackgroundColor;

    }
}
