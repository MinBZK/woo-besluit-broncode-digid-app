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
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.CardSteps.TA.RDW;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.TA
{
    public class TerminalAuthenticationRdw : BaseSecureCardOperation, IStep
    {
        internal TerminalAuthenticationRdw(ISecureCardOperation operation) : base((Gap)operation)
        {
        }

        public Certificate DvCert { get; set; }
        public ECPublicKeyParameters PublicKey { get; set; }
        public byte[] Ricc { get; set; }

        internal override void Init()
        {
            base.Init();

            Steps.Clear();
            Steps.Add(new StepSelectMF(this));
            Steps.Add(new StepSendPcaInfo(this));
            Steps.Add(new StepMseSetDst(this, false));
            // Verify the DVCA Certificate
            Steps.Add(new StepPsoVerifyCertificate(this, CertificateType.DVCACertificate));
            Steps.Add(new StepMseSetDst(this, true));
            // Verify the AT Certificate.
            Steps.Add(new StepPsoVerifyCertificate(this, CertificateType.ATCertificate));
            Steps.Add(new StepMseSetAT(this));
            Steps.Add(new StepGetChallenge(this));
            Steps.Add(new StepExternalAuthentication(this));
        }

        public override async Task<bool> Execute()
        {
            var result = await base.Execute();

            if (result)
                ((DrivingLicense)GAP.Card).TA = this;

            return result;
        }

        public override void StepCompleted(IStep stepNumber)
        {
            GAP.StepCompleted(stepNumber);
        }
    }
}
