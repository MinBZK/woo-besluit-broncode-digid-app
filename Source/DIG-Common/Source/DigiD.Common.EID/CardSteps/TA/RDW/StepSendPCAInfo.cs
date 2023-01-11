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
