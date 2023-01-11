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
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.PACE
{
    public class StepResetPace : IStep
    {
        private readonly PasswordType _passwordType;
        private readonly PaceOperation _pace;

        public StepResetPace(PasswordType passwordType, PaceOperation pace)
        {
            _passwordType = passwordType;
            _pace = pace;
        }
        public Task<bool> Execute()
        {
            _pace.PasswordType = _passwordType;
            return Task.FromResult(true);
        }
    }
}
