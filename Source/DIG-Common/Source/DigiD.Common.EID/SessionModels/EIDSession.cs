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
﻿using System.Net.Http;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Interfaces;

namespace DigiD.Common.EID.SessionModels
{
    public class EIDSession
    {
        public static void Init(INfcService nfcService, HttpClient httpClient, bool isDesktop)
        {
            NfcService = nfcService;
            Client = httpClient;
            IsDesktop = isDesktop;
        }

        public static INfcService NfcService { get; private set; }
        public static HttpClient Client { get; private set; }
        public static bool IsDesktop { get; private set; }
        public static Card Card { get; set; }

#if READ_PHOTO
        public static System.Security.SecureString MrzDrivingLicense { get; set; }
        public static System.Security.SecureString MrzIdentityCard { get; set; }
#endif
    }
}
