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
﻿using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <summary>
    /// Password Authenticated Connection Establishment
    /// PACE is performed to setup a secure channel between the terminal and the PCA.
    /// </summary>
    public class PaceOperation : BaseCardOperation, IStep
    {
        public bool IsSuccess => TerminalAuthToken != null && CardAuthToken != null && TerminalAuthToken.SequenceEqual(CardAuthToken) || Operation.ChangePinRequired;
        /// <summary>
        /// Only needed for online usages
        /// </summary>
        public AsymmetricCipherKeyPair EphemeralKeyPair { get; set; }
        public ECPublicKeyParameters CardPublicKey { get; set; }
        public AsymmetricCipherKeyPair AgreementKeyPair { get; set; } //K
        public ECPublicKeyParameters CardAgreedPublicKey { get; set; } 
        public byte[] DecryptedNonce { get; set; }
        public byte[] KEnc { get; set; }
        public byte[] KMac { get; set; }
        public byte[] TerminalAuthToken { get; set; }
        public byte[] TerminalToken { get; set; }
        public byte[] CardAuthToken { get; set; }
        public byte[] Car { get; set; }
        public PasswordType PasswordType { get; set; }
        public byte[] Password => Credentials.Password(PasswordType);

        internal readonly CardCredentials Credentials;
        private readonly bool _icao;
        protected Gap Operation { get; }

        internal PaceOperation(ISecureCardOperation operation,
            CardCredentials credentials,
            PasswordType passwordType, bool icao = false)
        {
            Operation = (Gap)operation;
            PasswordType = passwordType;
            Credentials = credentials;
            _icao = icao;
        }

		internal override void Init()
		{
            base.Init();

            Operation.Pace = this;

            switch (Operation.Card.DocumentType)
            {
                case DocumentType.DrivingLicense:
                    {
                        Steps.Add(new StepReadCardAccess(Operation));
                        Steps.Add(new StepSelectMF(Operation));

                        if (Operation.UserConsent != UserConsent.NotNeeded)
                            Steps.Add(new StepGetATCertificateRdw(Operation));
                        break;
                    }
                case DocumentType.IDCard:
                    {
                        if (_icao || PasswordType == PasswordType.MRZ)
                            Steps.Add(new StepSelecteDLApplication(Operation));
                        else
                            Steps.Add(new StepSelectPcaApplicationNik(Operation));

                        Steps.Add(new StepSelectMF(Operation));
                        Steps.Add(new StepStartSessionNik(Operation));
                        Steps.Add(new StepReadCardAccess(Operation));
                        break;
                    }
            }

            Steps.Add(new StepSetAT(Operation));

            if (Credentials != null)
            {
                Steps.Add(new StepGetEncryptedNonce(Operation));
                Steps.Add(new StepGenerateKeyPair(this, Operation));
                Steps.Add(new StepMapNonce(Operation));
                Steps.Add(new StepGenerateAgreementKeyPair(Operation));
                Steps.Add(new StepGetCardAgreementPublicKey(Operation));
                Steps.Add(new StepCalculateMacKeys(Operation));
                Steps.Add(new StepMutualAuthentication(Operation));
                Steps.Add(new StepValidateOperation(Operation));
            }
		}

		public override async Task<bool> Execute()
        {
            var result = await base.Execute() && (IsSuccess || Credentials == null);

            if (result)
            {
                Operation.Pace = this;
            }

            return result;
        }

        public override void StepCompleted(IStep stepNumber)
        {
            Operation.StepCompleted(stepNumber);
        }
    }
}
