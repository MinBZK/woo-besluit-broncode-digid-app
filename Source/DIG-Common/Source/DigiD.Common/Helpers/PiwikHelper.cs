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
using DigiD.Common.Constants;
using DigiD.Common.Services.Piwik;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class PiwikHelper
    {
        private static bool _init;
        public static async Task Init()
        {
            if (_init)
                return;

            _init = true;

            await PiwikTracker.Configure(AppConfigConstants.PiwikBaseUrl.ToString());
        }

        public static void TrackView(string viewName, string viewModel)
        {
            if (DependencyService.Get<IGeneralPreferences>().PiwikTrackEnabled)
            {
                PiwikTracker.Track(viewName, viewModel);
            }
        }

        public static void Track(string category, string action, string name)
        {
            if (DependencyService.Get<IGeneralPreferences>().PiwikTrackEnabled)
            {
                PiwikTracker.Track(category, action, name);
            }
        }
    }
}
