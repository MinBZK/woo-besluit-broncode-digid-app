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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.TA.RDW
{
    /// <summary>
    /// Send the DVCA or the TA Certificate to the PCA
    /// in preparation of the TA protocol
    /// See step 6, page 24 of the PCA implementation guidelines.
    /// </summary>
    internal class StepMseSetDst : BaseSecureStep, IStep
    {
        private readonly bool _needCertificate;

        public StepMseSetDst(ISecureCardOperation operation, bool needCertificate) : base(operation)
        {
            _needCertificate = needCertificate;
        }

        public async Task<bool> Execute()
        {
            var data = _needCertificate ? ((TerminalAuthenticationRdw)Operation).DvCert.HolderReference.Value : Operation.GAP.Pace.Car;
            var command = CommandApduBuilder.GetSetDSTCommand(data, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("TA MSESetDST", command, Operation.GAP.SMContext);
            
            return response.SW == 0x9000;
        }
    }
}
