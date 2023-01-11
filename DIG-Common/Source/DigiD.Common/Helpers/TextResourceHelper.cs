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
using System.Reflection;
using System.Resources;

namespace DigiD.Common.Helpers
{
    public static class TextResourceHelper
    {
        private const string ResourceId = "DigiD.Common.AppResources";

        internal static readonly Lazy<ResourceManager> Resmgr =
        new Lazy<ResourceManager>(() =>
            new ResourceManager(ResourceId, typeof(TextResourceHelper)
                    .GetTypeInfo().Assembly));

        public static string GetTextResource(string id)
        {
            var translation = Resmgr.Value.GetString(id, CultureInfo.DefaultThreadCurrentCulture);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{id}' was not found in resources '{ResourceId}' for culture '{CultureInfo.DefaultThreadCurrentCulture.Name}'.", nameof(id));
#endif
            }
            return translation;
        }
    }
}
