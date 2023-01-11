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
using System.Linq;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Helpers
{
    internal partial class CommandApduBuilder
    {
        internal static CommandApdu GetSetDSTCommand(byte[] car, SMContext context)
        {
            var carD0 = new DataObject(new byte[] { 0x83 }, car);
            return new CommandApdu(CLA.PLAIN, INS.MANAGE_SEC_ENV, (int)P1.SET_DST, (int)P2.COMPUTE_DIGITAL_SIGNATURE, carD0.GetEncoded()).Encrypt(context);
        }

        internal static CommandApdu[] GetPSOVerifyCertificateCommand(Certificate certificate, SMContext context)
        {
            var data = certificate.Body.Concat(certificate.Signature).ToArray();
            return new CommandApdu(CLA.PLAIN, INS.PERFORM_SEC_OP, (int)P1.SELECT_MF, (int)P2.CERTIFICATE, data).EncryptMulti(context);
        }

        internal static CommandApdu GetMSESetATCommand(byte[] publicKey, byte[] taInfoOID, byte[] cardHolderReference, SMContext context)
        {
            var cryptographicMechanismReference = new DataObject(new byte[] { 0x80 }, taInfoOID).GetEncoded();
            var publicKeyReference = new DataObject(new byte[] { 0x83 }, cardHolderReference).GetEncoded();
            var ephemeralPublicKey = new DataObject(new byte[] { 0x91 }, publicKey).GetEncoded();

            var data = cryptographicMechanismReference.Concat(publicKeyReference).Concat(ephemeralPublicKey).ToArray();

            return new CommandApdu(CLA.PLAIN, INS.MANAGE_SEC_ENV, (int)P1.SET_DST, (int)P2.SET_AT, data).Encrypt(context);
        }

        internal static CommandApdu GetChallengeCommand(SMContext context)
        {
            return new CommandApdu(CLA.PLAIN, INS.CHALLENGE, 0, 0, Array.Empty<byte>(), 8).Encrypt(context);
        }

        internal static CommandApdu GetExternalAuthenticate(byte[] signedData, SMContext context)
        {
            return new CommandApdu(CLA.PLAIN, INS.EXTERNAL_AUTHENTICATE, 0, 0, signedData).Encrypt(context);
        }
    }
}
