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
using System.Text;
using System.Threading.Tasks;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.Constants;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Exceptions;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.Demo
{
    internal class DemoStep : IStep
    {
        private readonly Gap _gap;
        private readonly CardState _state;
        private readonly CardOperationType _operationType;
        public bool ChangeTransportPin { get; set; }
        public bool ValidateCredentials { get; set; }
        public bool IsFinalStep { get; set; }
        
        public DemoStep(Gap gap, CardState state, CardOperationType operationType)
        {
            _gap = gap;
            _state = state;
            _operationType = operationType;
        }

        public async Task<bool> Execute()
        {
            var cmd = _gap.Card.DocumentType == DocumentType.DrivingLicense
                ? Convert.FromBase64String("SSSSSSSSSSSSSSSSSSSSSSSS")
                : Convert.FromBase64String("SSSSSSSSSSSSSSSS");

            var response = await EIDSession.NfcService.SendApduAsync(cmd);

            if (!response.Success)
                throw response.Exception;

            if (_gap.IsSuspended || _state.IsSuspended && _operationType != CardOperationType.ResumePin)
            {
                _state.NewPIN = null;
                _gap.IsSuspended = true;
                _gap.AuthenticationResult = AuthenticationResult.Failed;
                throw new CardSuspendException();
            }

            if (_gap.IsBlocked || _state.IsBlocked)
            {
                _gap.IsBlocked = true;
                _gap.AuthenticationResult = AuthenticationResult.Failed;
                throw new CardBlockedException();
            }

            if (ValidateCredentials)
            {
                var password = Encoding.UTF8.GetString(_gap.Pace.Password);
                string pin;

                if (_gap.PasswordType == PasswordType.PIN)
                {
                    pin = _state.PIN;

                    if (password != pin)
                    {
                        if (_state.IsSuspended)
                        {
                            _gap.IsSuspended = true;
                            _gap.AuthenticationResult = AuthenticationResult.Failed;
                            throw new CardSuspendException();
                        }
                    }
                    else if (_state.IsSuspended)
                    {
                        _gap.IsSuspended = true;
                        _gap.AuthenticationResult = AuthenticationResult.Failed;
                        throw new CardSuspendException();
                    }
                    else if (_state.IsBlocked)
                    {
                        _gap.IsBlocked = true;
                        _gap.AuthenticationResult = AuthenticationResult.Failed;
                        throw new CardBlockedException();
                    }

                    _gap.PinTriesLeft = _state.PINTries--;
                }
                else
                {
                    var delay = _state.CANDelay * 1.8;
                    if (delay > 20000)
                        delay = 200000;

                    _state.CANDelay = delay;
                    await Task.Delay((int)delay);
                    pin = _gap.Card.DocumentType == DocumentType.DrivingLicense ? WidConstants.CAN_RDW : WidConstants.CAN_NIK;
                }

                if (password == pin)
                {
                    _gap.ChangePinRequired = ChangeTransportPin;

                    if (ChangeTransportPin)
                    {
                        _gap.AuthenticationResult = AuthenticationResult.Success;
                        throw new TransportPinException();
                    }

                    if (_gap.PasswordType == PasswordType.CAN && _gap.Pace.Credentials.PIN.ToPlain() != _state.PIN)
                    {
                        _state.PINTries = 0;
                        _gap.IsBlocked = true;
                        _gap.AuthenticationResult = AuthenticationResult.Failed;
                        throw new CardBlockedException();
                    }

                    _gap.AuthenticationResult = AuthenticationResult.Success;
                    return true;
                }

                _gap.AuthenticationResult = AuthenticationResult.Failed;
                return false;
            }

            if (IsFinalStep && _gap.AuthenticationResult == AuthenticationResult.Success)
            {
                if (!string.IsNullOrEmpty(_state.NewPIN))
                {
                    _state.PIN = _state.NewPIN;
                    _state.NewPIN = null;
                }

                _gap.PinTriesLeft = null;
                _state.PINTries = 5;
                _state.CANDelay = 0;

                return true;
            }

            return response.Success;
        }
    }
}
