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
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.Models.Network.Responses;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;
using DigiD.Common.NFC.Helpers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DigiD.Common.EID.CardSteps.CAv2
{
    internal class StepSendFilesToServer : BaseSecureStep, IStep
    {
        internal StepSendFilesToServer(ISecureCardOperation operation) : base(operation)
        {

        }

        public async Task<bool> Execute()
        {
            var card = (eNIK)Operation.GAP.Card;

            var requestData = new
            {
                efCvca = card.CVCA.Base64Encoded,
                dg14 = card.DataGroups[14].Base64Encoded,
                efSOd = card.EF_SOd.Base64Encoded,
                paceIcc = Operation.GAP.Pace.CardAgreedPublicKey.Q.XCoord.GetEncoded().ToBase64()
            };

            var data = new EIDBaseRequest(requestData, Operation.GAP.SessionData.SessionId);

            var response = await EIDSession.Client.PostAsync<TerminalAuthenticationCommandResponses>(new Uri($"{Operation.GAP.SessionData.ServerAddress}{WidConstants.NIK_PREPARE_EAC_URI}"), data);

            if (response.ApiResult != ApiResult.Ok)
                return false;

            Operation.GAP.ApiResult = response.ApiResult;

            card.TACommands = response.Data.Commands;
            card.CardAuthenticationPublicKey = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(response.Data.EphemeralKey));

            return true;
        }
    }
}
