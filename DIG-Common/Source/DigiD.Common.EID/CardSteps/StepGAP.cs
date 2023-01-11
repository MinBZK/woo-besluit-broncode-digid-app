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
