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
using System.Security;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.CardSteps.PinManagement;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardOperations
{
    public class ChangePinOperation : BaseSecureCardOperation
    {
        private readonly SecureString _newPin;
        private readonly CardCredentials _credentials;
        private readonly PasswordType _passwordType;

        /// <summary>
        /// Change pin via CAN PACE
        /// </summary>
        /// <param name="newPin"></param>
        /// <param name="credentials"></param>
        /// <param name="passwordType"></param>
        /// <param name="progressChangedAction"></param>
        public ChangePinOperation(SecureString newPin, CardCredentials credentials, PasswordType passwordType, Action<float> progressChangedAction)
            : base(progressChangedAction)
        {
            _credentials = credentials;
            _passwordType = passwordType;
            _newPin = newPin;
        }

        public override async Task<bool> Execute()
        {
            if (await new StepGap(_credentials, _passwordType, GAPType.Pace, this, ChangeProgress)
                .Execute())
            {
                var result = await base.Execute();

                if (!result && GAP.ChangePinRequired)
                {
                    var crd = new StepChangeReferenceData(this, _newPin);
                    return await crd.Execute();
                }

                return result;
            }

            return false;
        }

        internal override void Init()
        {
            base.Init();

            if (GAP.Card.DocumentType == DocumentType.DrivingLicense)
            {
                Steps.Add(new StepSelectPcaApplicationRdw(this));
                Steps.Add(new StepSelectMF(this));
                Steps.Add(new StepResetRetryCounter(this, _newPin));
            }
            else
            {
                Steps.Add(new StepResetRetryCounter(this, _newPin));
            }
        }
    }
}
