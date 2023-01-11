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
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.CardSteps.PACE
{
    internal class StepValidateOperation : IStep
    {
        private readonly Gap _gap;

        public StepValidateOperation(Gap gap)
        {
            _gap = gap;
        }
        public Task<bool> Execute()
        {
            if (_gap.Pace.IsSuccess)
            {
                Debugger.WriteLine("PACE was successful");
                
                _gap.SMContext = new SMContext();
                _gap.SMContext.Init(_gap.Pace.KEnc, _gap.Pace.KMac);

                return Task.FromResult(true);
            }

            if (_gap.Card.DocumentType != DocumentType.IDCard && _gap.ChangePinRequired)
            {
                Debugger.WriteLine("PACE partial complete, need change Transport PIN");
                return Task.FromResult(true);
            }

            Debugger.WriteLine($"Terminal token: {_gap.Pace.TerminalAuthToken.ToHexString()}");
            Debugger.WriteLine($"Card token: {_gap.Pace.CardAuthToken.ToHexString()}");
            Debugger.WriteLine("PACE failed");

            return Task.FromResult(false);
        }
    }
}
