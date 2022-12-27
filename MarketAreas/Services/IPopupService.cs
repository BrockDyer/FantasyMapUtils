using System;
using CommunityToolkit.Maui.Views;

namespace MarketAreas.Services
{
	public interface IPopupService
	{
		/// <summary>
		/// Async call to display popup. Must be called on Dispatcher thread.
		/// </summary>
		/// <param name="popup">The popup to show.</param>
		/// <returns>A task representing the work of showing the popup, and
		/// containing the value returned by it.</returns>
		public Task<object> ShowPopupAsync(Popup popup);

		/// <summary>
		/// Display a popup.
		/// </summary>
		/// <param name="popup">The popup to display.</param>
		/// <returns>The result of the popup.</returns>
		public object ShowPopup(Popup popup);

        /// <summary>
        /// Display a popup and then execute a callback.
        /// </summary>
        /// <param name="popup">The popup to display.</param>
        /// <param name="callback">The action to execute when the popup is
        /// finished.
        /// </param>
        public void ShowPopup(Popup popup, Action<object> callback);
	}

	internal class PopupService : IPopupService
	{
        /// <summary>
        /// Displays a popup on the main page of the current application.
        /// </summary>
        /// <param name="popup">The popup to display</param>
        /// <returns>The result of the popup.</returns>
        public object ShowPopup(Popup popup)
        {
            return Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            {
                await ShowPopupAsync(popup);
            });
        }

        /// <summary>
        /// Display a popup on the main page of the current application and
        /// then execute a callback.
        /// </summary>
        /// <param name="popup">The popup to display.</param>
        /// <param name="callback">The action to execute when the popup is
        /// finished.
        /// </param>
        public void ShowPopup(Popup popup, Action<object> callback)
        {
            Application.Current.MainPage.Dispatcher.Dispatch(async () =>
            {
                var result = await ShowPopupAsync(popup);
                callback(result);
            });
        }

        /// <summary>
        /// Displays a popup on the main page of the current application.
        /// Must be called from Dispatcher thread.
        /// </summary>
        /// <param name="popup">The popup to display.</param>
        /// <returns>A task containing the result of the popup.</returns>
        public Task<object> ShowPopupAsync(Popup popup)
        {
            return Application.Current.MainPage.ShowPopupAsync(popup);
        }
    }
}

