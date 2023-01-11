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
