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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.EID.SessionModels;

namespace DigiD.Common.EID.Demo
{
    public class DemoCardOperation : BaseSecureCardOperation
    {
        private readonly CardState _state;
        public CardOperationType OperationType { get; }

        public DemoCardOperation(CardOperationType type, Action<float> progressChangedAction, CardCredentials credentials, PasswordType passwordType, CardState state) : base(progressChangedAction)
        {
            _state = state;
            GAP = new Gap(passwordType, credentials);
            OperationType = type;
        }

        internal override async void Init()
        {
            base.Init();
            var x = 0;

            var atr = await EIDSession.NfcService.GetATR();
            GAP.Card = CardHelper.GetCardByATR(atr);
            GAP.Card.EF_Dir = new EFDir(Array.Empty<byte>());
            
            EIDSession.Card = GAP.Card;

            switch (OperationType)
            {
                case CardOperationType.Authentication:
                    TotalSteps = GAP.Card.RandomizeSteps;
                    break;
                case CardOperationType.ChangePin:
                    _state.ChangePinRequired = false;
                    TotalSteps = GAP.Card.ChangePINSteps;
                    break;
                case CardOperationType.Status:
                    TotalSteps = GAP.Card.StatusSteps;
                    break;
                case CardOperationType.ResumePin:
                    TotalSteps = GAP.Card.ResumeSteps;
                    break;
            }

            while (x < TotalSteps)
            {
                x++;
                var step = new DemoStep(GAP, _state, OperationType);

                switch (x)
                {
                    case 6 when OperationType == CardOperationType.Authentication && _state.ChangePinRequired:
                        step.ValidateCredentials = true;
                        step.ChangeTransportPin = true;
                        break;
                    case 6 when (OperationType == CardOperationType.Authentication || OperationType == CardOperationType.ResumePin || OperationType == CardOperationType.ChangePin):
                        step.ValidateCredentials = true;
                        break;
                }

                Steps.Add(step);
            }

            Steps.Add(new DemoStep(GAP, _state, OperationType)
            {
                IsFinalStep = true
            });
        }
    }
}
