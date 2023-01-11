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
