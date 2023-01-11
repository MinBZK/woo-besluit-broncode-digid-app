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
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.SessionModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class RdaScanFailedViewModel : BaseViewModel
    {
#if A11YTEST
        public RdaScanFailedViewModel() : this(ActivationMethod.SMS)
        {

        }
#endif
        public string CancelText { get; set; }
        public AsyncCommand CancelCommand { get; set; }
        public string MessageText { get; set; }

        public RdaScanFailedViewModel(ActivationMethod activationMethod)
        {
            switch (activationMethod)
            {
                case ActivationMethod.RequestNewDigidAccount:
                    PageId = "AP087";
                    MessageText = AppResources.AP087_Message;
                    CancelText = AppResources.AP087_ButtonText;
                    break;
                case ActivationMethod.SMS:
                    PageId = "AP088";
                    MessageText = AppResources.AP088_Message;
                    CancelText = AppResources.AP088_ButtonText;
                    break;
                case ActivationMethod.App:
                case ActivationMethod.Password:
                    PageId = "AP089";
                    MessageText = AppResources.AP089_Message;
                    CancelText = AppResources.AP089_ButtonText;
                    break;
            }
            
            ButtonCommand = new AsyncCommand(async () =>
            {
                await NavigationService.GoBack();
            });
            
            CancelCommand = new AsyncCommand(async () =>
            {
                DialogService.ShowProgressDialog();

                switch (HttpSession.ActivationSessionData.ActivationMethod)
                {
                    case ActivationMethod.App:
                        await ActivationHelper.AskPushNotificationPermissionAsync(ActivationMethod.App);
                        break;
                    case ActivationMethod.RequestNewDigidAccount:
                        await DependencyService.Get<IEnrollmentServices>().SkipRda();
                        await ActivationHelper.AskPushNotificationPermissionAsync(ActivationMethod.RequestNewDigidAccount);
                        break;
                    default:
                        await ActivationHelper.StartActivationWithAlternative();
                        break;
                }

                DialogService.HideProgressDialog();
            });
        }
    }
}
