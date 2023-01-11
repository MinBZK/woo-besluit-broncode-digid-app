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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.CardFiles;

namespace DigiD.Common.EID.CardSteps.CAv1
{
    /// <summary>
    /// Send a MSE command to the PCA to set up CA
    /// See step 12, page 25 of the PCA implementation guidelines.
    /// </summary>
    internal class StepSetAT : BaseSecureStep, IStep
    {
        public StepSetAT(ISecureCardOperation operation) : base(operation)
        {
            
        }

        public async Task<bool> Execute()
        {
            var oid = ((DG14)Operation.GAP.Card.DataGroups[14]).ChipAuthenticationInfo.OID.GetEncoded().Skip(2).ToArray();
            var command = CommandApduBuilder.GetMSESetATCAv1Command(oid, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("CAv1 Set AT", command, Operation.GAP.SMContext);

            return response.SW == 0x9000;
        }
    }
}
