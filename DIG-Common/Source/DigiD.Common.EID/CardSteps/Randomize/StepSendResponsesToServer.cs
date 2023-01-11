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
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Constants;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.Models.Network.Responses;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;

namespace DigiD.Common.EID.CardSteps.Randomize
{
    /// <summary>
    /// Send the PCA responses from the previous step back to the server.
    /// The server returns "Ok" if it receives and processes the responses successfully.
    /// </summary>
    internal class StepSendResponsesToServer : BaseSecureStep, IStep
    {
        public StepSendResponsesToServer(ISecureCardOperation operation) : base(operation)
        {

        }

        public async Task<bool> Execute()
        {
            var requestData = new EIDBaseRequest(new
            {
                apduResponses = ((DrivingLicense)Operation.GAP.Card).ResponseAPDUs.Select(Convert.ToBase64String).ToArray()
            }, Operation.GAP.SessionData.SessionId);

            var response = await EIDSession.Client.PostAsync<SendSecureCommandsResponse>(new Uri(Operation.GAP.SessionData.ServerAddress + WidConstants.RDW_GET_POLYMORPHIC_DATA_URI), requestData);
            Operation.GAP.ApiResult = response.ApiResult;

            if (response.ApiResult == ApiResult.Ok)
                return response.Data.Result.ToUpperInvariant() == "OK";

            return false;
        }
    }
}
