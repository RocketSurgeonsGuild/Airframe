namespace Rocket.Surgery.Airframe.Popup.Confirmation
{
    public class ConfirmDetailModel
    {
        public ConfirmDetailModel(string message, string title = "Confirmation", string confirmMessage = "Ok", string declineMessage = "Cancel")
        {
            Message = message;
            Title = title;
            ConfirmMessage = confirmMessage;
            DeclineMessage = declineMessage;
        }

        /// <summary>
        /// Gets the confirmation message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the confirmation title.
        /// </summary>
        public string Title { get; }

        public string ConfirmMessage { get; }

        public string DeclineMessage { get; }
    }
}