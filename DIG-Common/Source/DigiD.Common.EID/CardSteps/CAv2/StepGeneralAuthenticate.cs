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
﻿using System.Linq;
using System.Threading.Tasks;
using BerTlv;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Enums;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.CAv2
{
    internal class StepGeneralAuthenticate : BaseSecureStep, IStep
    {
        private ECPublicKeyParameters _pk;

        public StepGeneralAuthenticate(ISecureCardOperation operation, ECPublicKeyParameters pk = null) : base(operation)
        {
            _pk = pk;
        }

        public async Task<bool> Execute()
        {
            if (_pk == null)
                _pk = ((eNIK)Operation.GAP.Card).CardAuthenticationPublicKey;

            var command = CommandApduBuilder.GetGeneralAuthenticate(_pk.Q.GetEncoded(), Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("CA General Authenticate", command, Operation.GAP.SMContext);

            if (response.SW != 0x9000)
                return false;

            if (Operation.GAP.Card.DocumentType == DocumentType.DrivingLicense)
            {
                var tlvs = Tlv.ParseTlv(response.Data);
                var data = tlvs.First().Children.ToList();

                var operation = (ChipAuthenticationRdw)Operation;

                operation.Ricc = data[0].Value;
                operation.Ticc = data[1].Value;
            }

            return true;
        }
    }
}
