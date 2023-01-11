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
