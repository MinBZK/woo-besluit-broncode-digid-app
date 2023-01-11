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
using System.Threading.Tasks;
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Constants;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.Models.Network.Responses;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;

namespace DigiD.Common.EID.CardSteps.PACE
{
    public class StepGetATCertificateRdw : IStep
    {
        private readonly Gap _gap;

        /// <summary>
        /// Will retrieve the AT certificate based on the UserConsent from the server
        /// </summary>
        /// <param name="gap"></param>
        public StepGetATCertificateRdw(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            if (_gap.SessionData.Consent == UserConsent.NotNeeded)
                throw new ArgumentException("UserConsent need to be set", nameof(_gap.UserConsent));

            var requestData = new
            {
                userConsentType = _gap.UserConsent == UserConsent.PP ? "PP" : "PIP",
                documentType = Convert.ToBase64String(_gap.Card.CardAID)
            };

            var response = await EIDSession.Client.PostAsync<AtCertificateResponse>(new Uri(_gap.SessionData.ServerAddress + WidConstants.RDW_GET_CERTIFICATE_URI), new EIDBaseRequest(requestData, _gap.SessionData.SessionId));
            _gap.ApiResult = response.ApiResult;

            if (response.ApiResult == ApiResult.Ok)
            {
                ((DrivingLicense)_gap.Card).ATCertificate = new Certificate(Convert.FromBase64String(response.Data.AtCertificate));
                return true;
            }

            return false;
        }
    }
}
