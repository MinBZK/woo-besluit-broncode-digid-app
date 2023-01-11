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
using System.Collections.Generic;
using DigiD.Common.Interfaces;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class AppCenterHelper
    {
        private const string CURRENT_PAGE_ID_KEY = "page";

        private static Dictionary<string, string> AddPageId(this Dictionary<string, string> props)
        {
            if (DependencyService.Get<INavigationService>() == null || string.IsNullOrEmpty(DependencyService.Get<INavigationService>().CurrentPageId))
                return props;

            if (props == null)
                props = new Dictionary<string, string>
                {
                    {CURRENT_PAGE_ID_KEY, DependencyService.Get<INavigationService>().CurrentPageId},
                };
            else
            {
                if (!props.ContainsKey(CURRENT_PAGE_ID_KEY))
                    props.Add(CURRENT_PAGE_ID_KEY, DependencyService.Get<INavigationService>().CurrentPageId);
            }

            return props;
        }

        public static void TrackError(Exception ex, Dictionary<string, string> props = null)
        {
            props = props.AddPageId();
            props.Add("network", Xamarin.Essentials.Connectivity.NetworkAccess.ToString());
            Crashes.TrackError(ex, props);
        }

        public static void TrackEvent(string name, Dictionary<string, string> props = null)
        {
            Analytics.TrackEvent(name, props.AddPageId());
        }
    }
}
