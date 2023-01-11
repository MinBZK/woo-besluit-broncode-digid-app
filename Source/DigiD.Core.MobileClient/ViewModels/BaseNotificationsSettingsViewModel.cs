// Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
// gericht is op transparantie en niet op hergebruik. Hergebruik van 
// de broncode is toegestaan onder de EUPL licentie, met uitzondering 
// van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
//
// This code has been disclosed in response to a request under the Dutch
// Open Government Act ("Wet open Overheid"). This implies that publication 
// is primarily driven by the need for transparence, not re-use.
// Re-use is permitted under the EUPL-license, with the exception 
// of source files that contain a different license.
//
// The archive that this file originates from can be found at:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// using the reference "Logius, publicly disclosed source code DigiD-App" 
//
// Other questions regarding this Open Goverment Act decision may be
// directed via email to open@logius.nl
//
﻿using System.Windows.Input;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using DigiD.UI.Popups;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class BaseNotificationsSettingsViewModel : BaseViewModel
    {
        private readonly bool _authShowPopupOnLoad;
        public ICommand OpenSettingsCommand { get; set; }
        public bool OpenSettingsCommandVisible { get; set; }

        private RemoteNotificationDisabledPopup _popup;

        public BaseNotificationsSettingsViewModel(bool authShowPopupOnLoad)
        {
            _authShowPopupOnLoad = authShowPopupOnLoad;
            OpenSettingsCommand = new Command(() =>
            {
                DependencyService.Get<IDevice>().OpenSettings();
            });

            if (Device.RuntimePlatform == Device.iOS) //Only subscribe for iOS, because OnAppearing will not be fired when app is resuming from background
            {
                MessagingCenter.Subscribe<App>(this, "OnResume", (a) =>
                {
                    var notificationsEnabled = DependencyService.Get<IPushNotificationService>().NotificationsEnabled();

                    if (notificationsEnabled && _popup.IsOpened)
                    {
                        _popup.Dismiss(null);
                    }

                    OpenSettingsCommandVisible = !notificationsEnabled && !_popup.IsOpened;
                });
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (_popup == null)
                _popup = new RemoteNotificationDisabledPopup();

            var notificationsDisabled = !DependencyService.Get<IPushNotificationService>().NotificationsEnabled();

            if (_authShowPopupOnLoad && !DependencyService.Get<IPreferences>().M17PopupShown && notificationsDisabled && !_popup.IsOpened)
            {
                DependencyService.Get<IPreferences>().M17PopupShown = true;
                NavigationService.OpenPopup(_popup);
            }
            else if (!notificationsDisabled && _popup.IsOpened)
                _popup.Dismiss(null);

            OpenSettingsCommandVisible = notificationsDisabled && !_popup.IsOpened;
        }
    }
}
