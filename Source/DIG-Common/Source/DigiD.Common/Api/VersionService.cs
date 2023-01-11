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
using DigiD.Common.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.ResponseModels;

namespace DigiD.Common.Api
{
    public class VersionService : IVersionService
    {
        /// <summary>
        /// Laat zien of de app versie die de app mee stuurt momenteel actief is, daarnaast bestaat er de mogelijkheid om per app versie een api version te kiezen.
        /// </summary>
        /// <returns>The version.</returns>
        public async Task<VersionCheckResponse> CheckVersion()
        {
            return await HttpHelper.GetAsync<VersionCheckResponse>("/apps/version", false, 5000);
        }
    }
}
