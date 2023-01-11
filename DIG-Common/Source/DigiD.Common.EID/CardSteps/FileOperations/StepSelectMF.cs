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
