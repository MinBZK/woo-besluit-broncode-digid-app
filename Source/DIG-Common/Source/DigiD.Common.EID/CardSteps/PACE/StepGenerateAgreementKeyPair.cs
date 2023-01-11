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
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <summary>
    /// The client generates a keypair based on the nonce of the previous step.
    /// </summary>
    internal class StepGenerateAgreementKeyPair : IStep
    {
        private readonly Gap _gap;

        public StepGenerateAgreementKeyPair(Gap gap)
        {
            _gap = gap;
        }

        public async Task<bool> Execute()
        {
            var agreementPair = await SecurityHelper.GenerateKeyPairAgreement(
                _gap.Pace.CardPublicKey,
                (ECPrivateKeyParameters)_gap.Pace.EphemeralKeyPair.Private,
                _gap.Pace.DecryptedNonce,
                _gap.Card.EF_CardAccess.PaceInfo.Algorithm);

            _gap.Pace.AgreementKeyPair = agreementPair;

            return true;
        }
    }
}
