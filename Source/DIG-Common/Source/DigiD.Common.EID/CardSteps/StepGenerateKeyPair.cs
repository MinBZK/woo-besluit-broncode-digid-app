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
using DigiD.Common.EID.CardSteps.PACE;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps
{
    /// <summary>
    /// Generate a public/private key pair for PACE and TA.
    /// </summary>
    internal class StepGenerateKeyPair : IStep
    {
        private readonly ICardOperation _operation;
        private readonly Gap _gap;

        public StepGenerateKeyPair(ICardOperation operation, Gap gap)
        {
            _operation = operation;
            _gap = gap;
        }

        public Task<bool> Execute()
        {
            switch (_operation)
            {
                case PaceOperation p:
                    p.EphemeralKeyPair = SecurityHelper.GenerateKeyPair(_gap.Card.EF_CardAccess.PaceInfo.Algorithm);
                    break;

            }

            return Task.FromResult(true);
        }
    }
}
