// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
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
