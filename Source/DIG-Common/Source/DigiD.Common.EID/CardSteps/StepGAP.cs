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
using DigiD.Common.EID.CardSteps.Randomize;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;

namespace DigiD.Common.EID.CardSteps
{
    public class StepGap : IStep
    {
        private readonly CardCredentials _credentials;
        private readonly PasswordType _passwordType;
        private readonly GAPType _type;
        private readonly BaseSecureCardOperation _operation;
        private readonly Action _progressChangedAction;
        private readonly EidSessionData _eidSessionData;

        public StepGap(
            CardCredentials credentials, 
            PasswordType passwordType, 
            GAPType type, 
            BaseSecureCardOperation operation,
            Action progressChangedAction, 
            EidSessionData eidSessionData = null)
        {
            _credentials = credentials;
            _passwordType = passwordType;
            _operation = operation;
            _progressChangedAction = progressChangedAction;
            _eidSessionData = eidSessionData;
            _type = type;
        }

        public async Task<bool> Execute()
        {
            _operation.GAP = new Gap(_operation, _credentials, _passwordType, _progressChangedAction, _eidSessionData);

            if (!await _operation.GAP.Init(_type))
                return false;

            if (!await _operation.GAP.Execute())
                return false;

            return true;
        }
    }
}
