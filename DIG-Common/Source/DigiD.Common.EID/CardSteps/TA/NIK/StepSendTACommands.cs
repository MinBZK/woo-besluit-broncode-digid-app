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
