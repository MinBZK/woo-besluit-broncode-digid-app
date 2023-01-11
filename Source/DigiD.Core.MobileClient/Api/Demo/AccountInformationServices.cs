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
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class AccountInformationServices : IAccountInformationServices
    {
        public async Task<TwoFactorResponseModel> GetTwoFactorStatus()
        {
            await Task.Delay(20);

            var result = new TwoFactorResponseModel();

            if (DependencyService.Get<IMobileSettings>().ActivationMethod == ActivationMethod.RequestNewDigidAccount)
            {
                result.ApiResult = ApiResult.Nok;
                result.ErrorMessage = "no_username_password";
                return result;
            }

            result.ApiResult = ApiResult.Ok;
            result.Enabled = DependencyService.Get<IDemoSettings>().TwoFactorEnabled ?? DemoHelper.CurrentUser.TwoFactorEnabled;

            return DemoHelper.Log("/apps/two_factor/get_two_factor", new BaseRequest(), result);
        }

        public async Task<AccountStatusResponse> GetAccountStatus()
        {
            await Task.Delay(20);

            return new AccountStatusResponse
            {
                ApiResult = ApiResult.Ok,
                EmailStatus = EmailStatus.Verified,
                UnreadMessagesCount = 3,
                TwoFactorEnabled = DependencyService.Get<IDemoSettings>().TwoFactorEnabled ?? false,
                ClassifiedDeceased = DebugConstants.IsClassifiedDeceased
            };
        }

        public async Task<ApiResult> SetTwoFactor(bool enabled)
        {
            await Task.Delay(20);
            DependencyService.Get<IDemoSettings>().TwoFactorEnabled = enabled;

            return DemoHelper.Log("/apps/two_factor/change_two_factor", new { enabled }, ApiResult.Ok);
        }
    }
}
