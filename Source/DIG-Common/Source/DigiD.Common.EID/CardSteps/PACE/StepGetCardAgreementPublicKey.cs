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
﻿using System.Threading.Tasks;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Apdu;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <summary>
    /// The client sends an APDU to the PCA,
    /// to obtain the Card Agreed public key.
    /// </summary>
    internal class StepGetCardAgreementPublicKey : IStep
    {
        private readonly Gap _gap;

        public StepGetCardAgreementPublicKey(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            var xy = SecurityHelper.GetXandYPoints((ECPublicKeyParameters)_gap.Pace.AgreementKeyPair.Public);

            var command = CommandApduBuilder.GetPairAgreementCommand(xy.x, xy.y, _gap.SMContext);
            var response = await CommandApduBuilder.SendAPDU("PACE GetCardAgreementPublicKey", command, _gap.SMContext);

            if (response.SW == 0x9000)
            {
                var agreementResp = new XYResponseAPDU(response);
                _gap.Pace.CardAgreedPublicKey = SecurityHelper.DecodeKey(agreementResp, _gap.Card.EF_CardAccess.PaceInfo.Algorithm);

                return true;
            }

            return false;
        }
    }
}
