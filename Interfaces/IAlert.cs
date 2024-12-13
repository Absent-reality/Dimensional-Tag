
namespace DimensionalTag
{
    public interface IAlert
    {
        /// <summary>
        /// Send a custom alert message.
        /// </summary>
        /// <param name="title">Title of the alert.</param>
        /// <param name="text">Message body.</param>
        /// <param name="cancel">Cancel button's text.</param>
        /// <param name="confirm">Confirm button's text.</param>
        /// <param name="isVisible">Visibility.</param>
        /// <returns>Bool on closing if confirmed.</returns>
        Task<bool> SendAlert(string title, string text, string cancel, string confirm, bool isVisible);
    }
}
