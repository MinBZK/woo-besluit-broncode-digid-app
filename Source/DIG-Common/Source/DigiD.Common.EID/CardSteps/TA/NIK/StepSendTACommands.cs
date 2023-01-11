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
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.Models.Network.Responses;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.CardSteps.TA.NIK
{
    internal class StepSendTACommands : BaseSecureStep, IStep
    {
        public StepSendTACommands(ISecureCardOperation operation) : base(operation)
        {
        }


        public async Task<bool> Execute()
        {
            var index = -1;
            ResponseApdu latestResponse = null;

            var card = (eNIK)Operation.GAP.Card;

            if (card.TACommands.Count == 0)
                return false;

            foreach (var commandString in card.TACommands)
            {
                index++;

                var bytes = Convert.FromBase64String(commandString);

                var command = new CommandApdu(bytes)
                    { IsSecureMessage = true};

                latestResponse = await CommandApduBuilder.SendAPDU($"APDUCommand {index}", command, null);

                if (latestResponse.SW != 0x9000)
                    break;
            }


            //if everything went fine and all commands are processed succesfull
            //de data of the latest command will represent the ricc which need to me signed by the server
            var requestData = new
            {
                counter = index,
                apdu = latestResponse != null ? Convert.ToBase64String(latestResponse.Bytes) : null
            };

            var response = await EIDSession.Client.PostAsync<PcaCommandsResponse>(new Uri($"{Operation.GAP.SessionData.ServerAddress}{WidConstants.NIK_PREPARE_PCA}"), new EIDBaseRequest(requestData, Operation.GAP.SessionData.SessionId));

            if (response.ApiResult != ApiResult.Ok)
                return false;

            Operation.GAP.ApiResult = response.ApiResult;
            card.PCACommands = response.Data.APDUCommandos;

            return true;
        }
    }
}
