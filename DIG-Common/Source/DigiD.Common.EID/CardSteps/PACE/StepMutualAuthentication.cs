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
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Enums;
using Org.BouncyCastle.Asn1;

namespace DigiD.Common.EID.CardSteps.PACE
{
    internal class StepMutualAuthentication : IStep
    {
        private readonly Gap _gap;

        public StepMutualAuthentication(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            var command = CommandApduBuilder.GetAuthCommand(_gap.Pace.TerminalToken, _gap.SMContext);
            var response = await CommandApduBuilder.SendAPDU("PACE MutualAuthentication", command, _gap.SMContext);

            if (response.SW1 == 0x63)
            {
                if (_gap.PinTriesLeft == null)
                {
                    switch (_gap.Pace.PasswordType)
                    {
                        case PasswordType.PIN:
                            _gap.PinTriesLeft = 5;
                            break;
                        case PasswordType.PUK:
                            _gap.PinTriesLeft = 10;
                            break;
                    }
                }
                else if (_gap.PinTriesLeft == 1)
                    _gap.IsBlocked = true;

                _gap.AuthenticationResult = AuthenticationResult.Failed;
            }
            else if (response.SW == 0x9000)
                _gap.AuthenticationResult = AuthenticationResult.Success;
            else if (_gap.Card.DocumentType == DocumentType.IDCard && response.SW == 0x6200)
            {
                _gap.ChangePinRequired = true;
                return true;
            }

            return response.SW == 0x9000 && ParseData(response.Data);
        }

        private bool ParseData(byte[] data)
        {
            using (var s = new Asn1InputStream(data))
            {
                var dynamicAuthData = (DerApplicationSpecific)s.ReadObject();
                var authRespSet = Asn1Set.GetInstance(dynamicAuthData.GetObject(0x11));

                foreach (Asn1TaggedObject element in authRespSet)
                {
                    var val = Asn1OctetString.GetInstance(element).GetOctets();

                    switch (element.TagNo)
                    {
                        case 6:
                            _gap.Pace.CardAuthToken = val;
                            break;
                        case 7:
                            _gap.Pace.Car = val;
                            break;
                    }
                }

                return true;
            }
        }
    }
}
