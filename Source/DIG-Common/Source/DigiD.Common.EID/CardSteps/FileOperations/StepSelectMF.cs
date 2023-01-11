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
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardSteps.FileOperations
{
    /// <summary>
    /// Select the Master File on the PCA
    /// </summary>
    internal class StepSelectMF : IStep
    {
        private Gap _gap;
        private readonly ISecureCardOperation _operation;
        
        public StepSelectMF(ISecureCardOperation operation)
        {
            _operation = operation;
        }

        public StepSelectMF(Gap gap)
        {
            _gap = gap;
        }

        public async Task<bool> Execute()
        {
            if (_gap == null)
                _gap = _operation.GAP;

            var command = CommandApduBuilder.GetMFSelectCommand(_gap.SMContext, _gap.Card.DocumentType == DocumentType.IDCard);
            
            var response = await CommandApduBuilder.SendAPDU("Select MasterFile", command, _gap.SMContext);

            return response.SW == 0x9000 || response.SW == 0x6982;
        }
    }
}
