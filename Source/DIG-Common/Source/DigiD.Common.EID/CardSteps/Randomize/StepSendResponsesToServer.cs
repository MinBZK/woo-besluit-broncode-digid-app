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
