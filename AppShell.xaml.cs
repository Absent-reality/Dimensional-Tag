using System.Windows.Input;

namespace DimensionalTag
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ICommand CenterViewCommand { get; } = new Command(async () => await Current.Navigation.PushModalAsync(new SearchPage()));

    }
}
