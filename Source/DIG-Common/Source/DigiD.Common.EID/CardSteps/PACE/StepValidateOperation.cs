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
