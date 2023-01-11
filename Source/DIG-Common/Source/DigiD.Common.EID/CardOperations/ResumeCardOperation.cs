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
﻿using System;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.CardSteps.PACE;
using DigiD.Common.EID.CardSteps.PinManagement;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardOperations
{
    public class ResumeOperation : BaseSecureCardOperation
    {
        private readonly CardCredentials _credentials;
        private readonly GAPType _type;


        /// <summary>
        /// Starts a first a PACE session with CAN, after a successful authentication, a new PACE session will be initiated with <param name="type"></param>
        /// </summary>
        /// <param name="type">Reset PIN or PUK</param>
        /// <param name="credentials"></param>
        /// <param name="progressChangedAction"></param>
        public ResumeOperation(GAPType type, CardCredentials credentials, Action<float> progressChangedAction) : base(progressChangedAction)
        {
            _credentials = credentials;
            _type = type;
        }

        public override async Task<bool> Execute()
        {
            if (await new StepGap(_credentials, PasswordType.CAN, GAPType.Pace, this, ChangeProgress).Execute())
                return await base.Execute();

            return false;
        }

        internal override void Init()
        {
            base.Init();

            switch (GAP.Card.DocumentType)
            {
                case DocumentType.IDCard:
                    switch (_type)
                    {
                        case GAPType.PINResumeCAN:
                            Steps.Add(new StepVerify(this, _credentials, PasswordType.PIN));
                            break;
                        case GAPType.PUKResumeCAN:
                            Steps.Add(new StepVerify(this, _credentials, PasswordType.PUK));
                            break;
                    }

                    break;
                case DocumentType.DrivingLicense:
                    switch (_type)
                    {
                        case GAPType.PINResumeCAN:
                            Steps.Add(new StepResetPace(PasswordType.PIN, GAP.Pace));
                            break;
                        case GAPType.PUKResumeCAN:
                            Steps.Add(new StepResetPace(PasswordType.PUK, GAP.Pace));
                            break;
                    }

                    Steps.Add(new StepSetAT(GAP));
                    Steps.Add(new StepGetEncryptedNonce(GAP));
                    Steps.Add(new StepGenerateKeyPair(this, GAP));
                    Steps.Add(new StepMapNonce(GAP));
                    Steps.Add(new StepGenerateAgreementKeyPair(GAP));
                    Steps.Add(new StepGetCardAgreementPublicKey(GAP));
                    Steps.Add(new StepCalculateMacKeys(GAP));
                    Steps.Add(new StepMutualAuthentication(GAP));
                    Steps.Add(new StepValidateOperation(GAP));
                    break;
            }
        }
    }
}
