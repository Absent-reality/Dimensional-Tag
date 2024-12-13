
using CommunityToolkit.Maui.Views;

namespace DimensionalTag
{
    public class MauiAlert : IAlert
    {
        /// <inheritdoc/>
        public async Task<bool> SendAlert(string title, string text, string cancel, string confirm, bool isVisible)
        {
           var result = await Shell.Current.ShowPopupAsync(new AlertPopup(title, text, cancel, confirm, isVisible));
            if(result is bool isConfirmed)
            return isConfirmed;
            else return false;
        }
    }
}
