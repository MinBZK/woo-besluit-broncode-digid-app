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
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;

namespace DigiD.Common.EID.CardSteps.TA.RDW
{
    /// <summary>
    /// Verify either the DVCA or TA Certificate
    /// See step 6, page 24 of the PCA implementation guidelines.
    /// </summary>
    internal class StepPsoVerifyCertificate : BaseSecureStep, IStep
    {
        private readonly CertificateType _certType;

        public StepPsoVerifyCertificate(ISecureCardOperation operation, CertificateType certificateType) : base(operation)
        {
            _certType = certificateType;
        }

        public async Task<bool> Execute()
        {
            var cert = _certType == CertificateType.DVCACertificate ? ((TerminalAuthenticationRdw)Operation).DvCert : ((DrivingLicense)Operation.GAP.Card).ATCertificate;
            
            var commands = CommandApduBuilder.GetPSOVerifyCertificateCommand(cert, Operation.GAP.SMContext);

            for (var x = 0; x <= commands.Length - 1; x++)
            {
                var command = commands[x];
                var response = await CommandApduBuilder.SendAPDU("TA PSOVerifyCertificate", command, Operation.GAP.SMContext, false);

                if (response.SW != 0x9000)
                    return false;

                if (x != commands.Length - 1) continue;

                response = response.DecryptAES(Operation.GAP.SMContext);
                return response.SW == 0x9000;
            }

            return false;
        }
    }
}
