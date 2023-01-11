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
