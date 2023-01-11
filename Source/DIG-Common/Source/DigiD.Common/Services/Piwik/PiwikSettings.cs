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
using DigiD.Common.BaseClasses;
using DigiD.Common.Interfaces;
using DigiD.Common.Services.Piwik;

[assembly: Xamarin.Forms.Dependency(typeof(PiwikSettings))]
namespace DigiD.Common.Services.Piwik
{
    /// <summary>
    /// Piwik settings. Do not use this class directly as it is only intended to use internally.
    /// </summary>
    public class PiwikSettings : BaseAppSettings, IPiwikSettings
    {
        public int SessionsCount
        {
            get => GetValue(0);
            set => SetValue(value);
        }

        public DateTime FirstVisit
        {
            get => GetValue(DateTime.Now);
            set => SetValue(value);
        }

        public DateTime LastVisit
        {
            get => GetValue(DateTime.Now);
            set => SetValue(value);
        }
    }
}
