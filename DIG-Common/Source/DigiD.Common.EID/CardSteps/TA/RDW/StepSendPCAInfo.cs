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
using DigiD.Common.EID.Constants;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.Models.Network.Responses;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DigiD.Common.EID.CardSteps.TA.RDW
{
    /// <summary>
    /// Send the
    /// - CARs
    /// - PACE ephemeral public key
    /// - the contents of EfCardAccess to the server.
    /// See step 2 on page 23 of the PCA implementation guidelines. (Remote exchange 1)
    /// </summary>
    internal class StepSendPcaInfo : BaseSecureStep, IStep
    {
        public StepSendPcaInfo(ISecureCardOperation operation) : base(operation)
        {
            
        }

        public async Task<bool> Execute()
        {
            var msgData = new
            {
                car = Convert.ToBase64String(Operation.GAP.Pace.Car),
                idpicc = Convert.ToBase64String(Operation.GAP.Pace.CardAgreedPublicKey.Q.XCoord.GetEncoded()),
                efCardAccess = Convert.ToBase64String(Operation.GAP.Card.EF_CardAccess.Bytes)
            };

            var requestData = new EIDBaseRequest(msgData, Operation.GAP.SessionData.SessionId);

            var response = await EIDSession.Client.PostAsync<PcaInfoServerResponse>(new Uri(Operation.GAP.SessionData.ServerAddress + WidConstants.RDW_POLYMORPHIC_INFO_URI), requestData);
            Operation.GAP.ApiResult = response.ApiResult;

            if (response.ApiResult == ApiResult.Ok)
            {
                var ephemeralKey = Convert.FromBase64String(response.InfoResponse.EphemeralKey);

                ((TerminalAuthenticationRdw)Operation).PublicKey = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(ephemeralKey);
                ((TerminalAuthenticationRdw)Operation).DvCert = new Certificate(Convert.FromBase64String(response.InfoResponse.DvCertificate));

                return true;
            }

            return false;              
        }
    }
}
