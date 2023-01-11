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
