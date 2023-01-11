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
﻿using System.Linq;
using System.Threading.Tasks;
using BerTlv;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardSteps.PACE
{
    internal class StepGetEncryptedNonce : IStep
    {
        private readonly Gap _gap;

        /// <summary>
        /// The PCA randomly generates a nonce.
        /// See BSI worked example 3.2, page 16
        /// </summary>
        public StepGetEncryptedNonce(Gap gap)
        {
            _gap = gap;
        }

        public async Task<bool> Execute()
        {
            var command = CommandApduBuilder.GetNonceCommand(_gap.SMContext);
            var response = await CommandApduBuilder.SendAPDU("PACE GetEncryptedNonce", command, _gap.SMContext);

            if  (response.SW == 0x9000)
            {
                return ParseData(response.Data);
            }

            return false;
        }

        private bool ParseData(byte[] data)
        {
            var tlvs = Tlv.ParseTlv(data);
            var tlv = tlvs.FirstOrDefault();

            tlv = tlv?.FindTag("80");

            if (tlv == null)
                return false;

            var password = _gap.Pace.Password;

            if (_gap.Pace.PasswordType == PasswordType.MRZ && _gap.Card.DocumentType != DocumentType.DrivingLicense)
                password = CryptoDesHelper.ComputeSHA1Hash(password);

            var pin = password.Concat(new byte[] { 0x00, 0x00, 0x00, 0x03 }).ToArray();
            var pinBytes = AesHelper.CalculateHash(pin, _gap.Card.MessageDigestAlgorithm, _gap.Card.KeyLength);

            _gap.Pace.DecryptedNonce = AesHelper.AESCBC(false, tlv.Value, pinBytes);

            return true;
        }
    }
}
