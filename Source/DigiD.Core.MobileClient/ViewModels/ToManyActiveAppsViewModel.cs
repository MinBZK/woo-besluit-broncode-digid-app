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
﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models.ResponseModels;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DigiD.ViewModels
{
    public class ToManyActiveAppsViewModel : BaseViewModel
    {
        public string AppInformation { get; set; }

        public IAsyncCommand CancelCommand { get; set; }

#if A11YTEST
        public ToManyActiveAppsViewModel() : this(3) { }
#endif

        public ToManyActiveAppsViewModel(ActivationResponse model, Func<Task> confirm)
        {
            PageId = "AP094";
            FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.AP094_Message, model.MaxAmount);
            AppInformation = string.Format(CultureInfo.InvariantCulture,AppResources.AP094_AppInformation, model.DeviceName, model.LastUsed);

            CancelCommand = new AsyncCommand(async () =>
            {
                await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.ActivationFailedTooManyDevices));
            });

            ButtonCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopCurrentModalPage();
                await confirm.Invoke();
            });
        }

        public ToManyActiveAppsViewModel(int maxAmount)
        {
            PageId = "AP200";
            FooterText = string.Format(AppResources.AP200_ActivationConfirmTooManyDevicesMessage, maxAmount);

            ButtonCommand = new AsyncCommand(async () =>
            {
                await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.ActivationCancelled));
            });
        }
    }
}
