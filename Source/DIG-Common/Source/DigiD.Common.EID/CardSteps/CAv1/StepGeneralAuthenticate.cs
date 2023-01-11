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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.CAv1
{
    internal class StepGeneralAuthenticate : BaseSecureStep, IStep
    {
        public StepGeneralAuthenticate(ISecureCardOperation operation) : base(operation)
        {
        }

        public async Task<bool> Execute()
        {
            // var pk = Operation.GAP.Pace.Credentials.RDEData.EphemeralPublicKey.GetEncoded(false);
            var pk = new byte[]{};
            var command = CommandApduBuilder.GetGeneralAuthenticateCAv1(pk, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("CAv1 General Authenticate", command, Operation.GAP.SMContext);

            if (response.SW == 0x9000)
            {
                var skPCD = (ECPrivateKeyParameters)Operation.GAP.Pace.EphemeralKeyPair.Private;
                var pkPICC = SecurityHelper.DecodeKey(((DG14)Operation.GAP.Card.DataGroups[14]).ChipAuthenticationPublicKeyInfo.Pk, ChipAuthenticationPublicKeyInfo.Algorithm);

                //Generate new SharedSecret by card private key, and EF_DG14 public key
                //Get X coordinate to user for kEnc enkMac generation
                var sharedSecretX = SecurityHelper.GenerateSharedSecret(skPCD, pkPICC).AffineXCoord.GetEncoded();

                var kEnc = SecurityHelper.CalculateKEnc(sharedSecretX, Operation.GAP.Card);
                var kMac = SecurityHelper.CalculateKMac(sharedSecretX, Operation.GAP.Card);

                Operation.GAP.SMContext.Init(kEnc, kMac);

                return true;
            }

            return false;
        }
    }
}
