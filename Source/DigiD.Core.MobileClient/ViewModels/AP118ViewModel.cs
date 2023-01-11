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
﻿using DigiD.Common;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class BaseTwoFactorViewModel : BaseViewModel
    {
        public bool TwoFactorEnabled { get; set; } = true;
        private bool _undo;

        public AsyncCommand<bool> TwoFactorChangedCommand => new AsyncCommand<bool>(async value =>
        {
            if (_undo)
                return;

            if (!value)
            {
                var result = await DependencyService.Get<IAlertService>().DisplayAlert(
                    AppResources.M18_Header
                    , AppResources.M18_Message
                    , AppResources.Yes
                    , AppResources.No, true);

                if (!result)
                {
                    _undo = true;
                    TwoFactorEnabled = true;
                    _undo = false;

                    return;
                }
            }

            await DependencyService.Get<IAccountInformationServices>().SetTwoFactor(TwoFactorEnabled);
        });
    }

    public class AP118ViewModel : BaseTwoFactorViewModel
    {
        private readonly RegisterEmailModel _model;

        public bool NotificationsEnabled { get; set; } = true;
        public bool TwoFactorVisible { get; }
        public bool PushNotificationsVisible { get; }
        
        public AP118ViewModel(bool twoFactorEnabled, RegisterEmailModel model)
        {
            PageId = "AP118";
            
            _model = model;
            ButtonCommand = NextCommand;
            TwoFactorVisible = !twoFactorEnabled;
            PushNotificationsVisible = DependencyService.Get<IPushNotificationService>().NotificationsAvailable;
        }

        private AsyncCommand NextCommand => new AsyncCommand(async () =>
        {
            if (PushNotificationsVisible)
                await DependencyService.Get<IAccountInformationServices>().SetTwoFactor(TwoFactorEnabled);

            await RemoteNotificationHelper.RequestToken(NotificationsEnabled);
            await ActivationHelper.RegisterEmailTaskAsync(_model);
        });
    }
}
