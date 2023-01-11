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
