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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.TA.RDW
{
    /// <summary>
    /// Request a random challenge of the PCA.
    /// This challenge is to be send to the server.
    /// See page 8, page 25 of the PCA implementation guidelines.
    /// </summary>
    internal class StepGetChallenge : BaseSecureStep, IStep
    {
        public StepGetChallenge(ISecureCardOperation operation) : base(operation)
        {
        }

        public async Task<bool> Execute()
        {
            var command = CommandApduBuilder.GetChallengeCommand(Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("TA GetChallenge", command, Operation.GAP.SMContext);

            if (response.SW == 0x9000)
            {
                ((TerminalAuthenticationRdw)Operation).Ricc = response.Data;
                return true;
            }

            return false;
        }
    }
}
