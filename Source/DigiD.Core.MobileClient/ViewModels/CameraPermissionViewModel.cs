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
﻿using System.Threading.Tasks;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Services;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class CameraPermissionViewModel : BaseViewModel
    {
        private bool _initialized;
        private readonly bool _eIDAuthentication;

#if A11YTEST
        public CameraPermissionViewModel() : this(false) { }
#endif

        public CameraPermissionViewModel(bool eIDAuthentication)
        {
            PageId = "AP003";
            _eIDAuthentication = eIDAuthentication;
            ButtonCommand = new AsyncCommand(async () => await RequestPermission());
            
            NavCloseCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot();
            });
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_initialized)
                return;

            _initialized = true;

            await RequestPermission();
        }

        private async Task RequestPermission()
        {
            var requestResult = await Permissions.RequestAsync<Permissions.Camera>();

            if (requestResult == PermissionStatus.Granted)
            {
                await QRCodeScannerHelper.ShowScannerPage(false, _eIDAuthentication);
            }
            else
                DependencyService.Get<IDevice>().OpenSettings();
        }
    }
}
