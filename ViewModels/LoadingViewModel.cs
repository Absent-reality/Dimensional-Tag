
namespace DimensionalTag
{
    public partial class LoadingViewModel(AppSettings settings, IAlert alert) : BaseViewModel(settings, alert)
    {
        public IAlert Alerts { get; set; } = alert;
        public AppSettings AppSettings { get; set; } = settings;
    }
}
