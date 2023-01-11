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
using DigiD.Common.Controls;
using DigiD.Common.Services;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
namespace DigiD.Common.Services
{
    public class AlertService : IAlertService
    {
        public async Task<bool> DisplayAlert(string title, string message, string accept = null, string cancel = null, bool invertCallToAction = false)
        {
            try
            {
                if (cancel == null)
                {
                    await Application.Current.MainPage.Navigation.ShowPopupAsync(new AlertView(title, message, accept ?? AppResources.OK, cancel, invertCallToAction));
                    return await Task.FromResult(true);
                }

                var result = await Application.Current.MainPage.Navigation.ShowPopupAsync(new AlertView(title, message, accept ?? AppResources.OK, cancel, invertCallToAction));
                if (result is bool resultAsBool)
                    return await Task.FromResult(resultAsBool);

                return await Task.FromResult(false);
            }
            catch
            {
                return false;
            }
        }
    }
}
