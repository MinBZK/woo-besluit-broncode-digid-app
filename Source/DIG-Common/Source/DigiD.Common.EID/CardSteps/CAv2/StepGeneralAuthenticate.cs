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
