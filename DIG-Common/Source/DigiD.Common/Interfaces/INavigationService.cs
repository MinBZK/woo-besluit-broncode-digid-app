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
using DigiD.Common.BaseClasses;
using DigiD.Common.Enums;
using DigiD.Common.Models;
using Xamarin.Forms;

namespace DigiD.Common.Interfaces
{
    public interface INavigationService
    {
        Page GetPage(CommonBaseViewModel viewModel);
        void SetPage(CommonBaseViewModel viewModel);
        Task PushAsync(CommonBaseViewModel viewModel, bool animate = true);
        Task<bool> ConfirmAsync(string confirmAction, RegisterEmailModel model = null);
        Task PushModalAsync(CommonBaseViewModel viewModel, bool animate = true, bool nav = true);
        Task GoBack(bool animate = true);
        Task PopCurrentModalPage(bool animate = true);
        Task ShowMessagePage(MessagePageType pageType, object data = null);
        Task GoToPincodePage(PinCodeModel model, bool animate = true);
        Task GoToNFCScannerPage(NfcScannerModel model);
        Task PopToRoot(bool force = false);
        Task ResetMainPage(params CommonBaseViewModel[] viewModels);
        string CurrentPageId { get; }
        Task<T> OpenPopup<T>(BasePopup<T> popup);
        void OpenPopup(BasePopup popup);
#if A11YTEST
        System.Collections.Generic.Dictionary<System.Type, System.Type> RegisteredPages { get; }
#endif
    }
}
