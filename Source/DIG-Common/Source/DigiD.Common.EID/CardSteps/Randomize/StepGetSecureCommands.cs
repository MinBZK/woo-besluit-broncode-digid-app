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
    /// The client sends the contents of EfCardSecurity and
    /// the PCA responses from the previous step (Ricc and Ticc) to the server.
    /// See step 13, page 25 of the PCA implementation guidelines.
    /// 
    /// The Server generates 3 secured APDU's and sends these back to the client.
    /// For the steps on the server see steps 14 - 17, page 25 of the PCA implementation guidelines.
    /// </summary>
    internal class StepGetSecureCommands : BaseSecureStep, IStep
    {
        public StepGetSecureCommands(ISecureCardOperation operation): base(operation)
        {
            
        }

        public async Task<bool> Execute()
        {
            var card = (DrivingLicense) Operation.GAP.Card;

            var efCardSecurity = Convert.ToBase64String(card.CardSecurity.Bytes);
            var pcaApplicationID = Convert.ToBase64String(Operation.GAP.Card.PolymorphicAID);

            var msgData = new
            {
                efCardSecurity,
                pcaApplicationId = pcaApplicationID,
                rpicc = Convert.ToBase64String(card.CA.Ricc),
                tpicc = Convert.ToBase64String(card.CA.Ticc),
            };

            var requestData = new EIDBaseRequest(msgData, Operation.GAP.SessionData.SessionId);
            
            var response = await EIDSession.Client.PostAsync<SetupSecureMessagingResponse>(new Uri(Operation.GAP.SessionData.ServerAddress + WidConstants.RDW_SECURE_APDU_URI), requestData);
            Operation.GAP.ApiResult = response.ApiResult;

            if (response.ApiResult == ApiResult.Ok)
            {
                ((DrivingLicense)Operation.GAP.Card).CommandAPDUs = response.Data.Commands;
                return true;
            }

            return false;
        }
    }
}
