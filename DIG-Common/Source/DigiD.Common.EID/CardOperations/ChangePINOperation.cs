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
