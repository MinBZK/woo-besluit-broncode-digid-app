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
﻿using System;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardOperations;
using DigiD.Common.EID.CardSteps.BAC;
using DigiD.Common.EID.CardSteps.CAv1;
using DigiD.Common.EID.CardSteps.CAv2;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.CardSteps.PACE;
using DigiD.Common.EID.CardSteps.Randomize;
using DigiD.Common.EID.CardSteps.TA;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardSteps
{
    public class Gap : BaseSecureCardOperation
    {
        private readonly BaseSecureCardOperation _operation;
        private readonly CardCredentials _credentials;
        private readonly Action _progressChangedAction;

        public readonly PasswordType PasswordType;
        internal PaceOperation Pace { get; set; }
        internal SMContext SMContext { get; set; }
        public Card Card { get; set; }
        public AuthenticationResult AuthenticationResult { get; set; }
        public ApiResult ApiResult { get; set; }

        public int? PinTriesLeft { get; set; }
        public bool ChangePinRequired { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSuspended { get; set; }
        internal EidSessionData SessionData { get; }
        internal UserConsent UserConsent => SessionData?.Consent ?? UserConsent.NotNeeded;

        public Gap(PasswordType passwordType, CardCredentials cardCredentials)
        {
            PasswordType = passwordType;
            Pace = new PaceOperation(this, cardCredentials, passwordType);
        }

        public Gap(
            BaseSecureCardOperation operation,
            CardCredentials credentials,
            PasswordType passwordType,
            Action progressChangedAction, 
            EidSessionData sessionData)
        {
            _operation = operation;
            _credentials = credentials;
            PasswordType = passwordType;
            _progressChangedAction = progressChangedAction;
            SessionData = sessionData;
        }

        public Gap(CardCredentials credentials, PasswordType passwordType, BaseSecureCardOperation operation)
        {
            _credentials = credentials;
            PasswordType = passwordType;
            _operation = operation;
        }

        public async Task<bool> Init(GAPType type)
        {
            var atr = await EIDSession.NfcService.GetATR();
            _operation.GAP.Card = CardHelper.GetCardByATR(atr);
            EIDSession.Card = _operation.GAP.Card;

            if (type == GAPType.Bac)
            {
#if READ_PHOTO
                if (_credentials.MRZ == null)
                {

                    _credentials.MRZ = _operation.GAP.Card.DocumentType == DocumentType.DrivingLicense 
                        ? EIDSession.MrzDrivingLicense 
                        : EIDSession.MrzIdentityCard;
                }
#endif

                Steps.Add(new BacOperation(this, _credentials.MRZ));
                Steps.Add(new StepPassiveAuthentication(this));
                Steps.Add(new StepActiveAuthentication(this));

                switch (_operation)
                {
                    case ReadPhotoOperation rp:
                        rp.TotalSteps = 5;
                        break;
                }

                return true;
            }

            switch (_operation)
            {
                case ChangePinOperation cp:
                    cp.TotalSteps = _operation.GAP.Card.ChangePINSteps;
                    break;
                case StatusOperation cs:
                    cs.TotalSteps = _operation.GAP.Card.StatusSteps;
                    break;
                case RandomizeOperation r:
                    r.TotalSteps = _operation.GAP.Card.RandomizeSteps;
                    break;
                case ResumeOperation resume:
                    resume.TotalSteps = _operation.GAP.Card.ResumeSteps;
                    break;
                case ReadPhotoOperation rpo:
                    rpo.TotalSteps = 20;
                    break;
            }

            Steps.Add(new StepSelectMF(this));
            Steps.Add(new StepReadEFDir(this));

            switch (type)
            {
                case GAPType.Pace:
                    Steps.Add(new PaceOperation(this, _credentials, PasswordType, _operation is ReadPhotoOperation));
                    break;
                case GAPType.PaceStatus:
                    Steps.Add(new PaceOperation(this, null, PasswordType));
                    break;
                case GAPType.PINResumeCAN:
                    Steps.Add(new PaceReset(this, _credentials, PasswordType, PasswordType.PIN));
                    break;
                case GAPType.PUKResumeCAN:
                    Steps.Add(new PaceReset(this, _credentials, PasswordType, PasswordType.PUK));
                    break;
                case GAPType.CAv1:
                    Steps.Add(new PaceOperation(this, _credentials, PasswordType));
                    Steps.Add(new StepSelecteDLApplication(this));
                    Steps.Add(new StepReadFiles(this, _operation is ReadPhotoOperation));
                    Steps.Add(new ChipAuthentication(this));
                    break;
                case GAPType.CAv2:
                    if (_operation.GAP.Card.DocumentType == DocumentType.DrivingLicense)
                    {
                        Steps.Add(new PaceOperation(this, _credentials, PasswordType));
                        Steps.Add(new StepSelectPcaApplicationRdw(this));
                        Steps.Add(new TerminalAuthenticationRdw(this));
                        Steps.Add(new ChipAuthenticationRdw(this));
                    }
                    else
                    {
                        Steps.Add(new PaceOperation(this, _credentials, PasswordType));
                        Steps.Add(new StepSelectPcaApplicationNik(this));
                        Steps.Add(new ChipAuthenticationNik(this));
                        Steps.Add(new TerminalAuthenticationNik(this));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return true;
        }

        public override void StepCompleted(IStep stepNumber)
        {
            _progressChangedAction?.Invoke();
        }
    }
}
