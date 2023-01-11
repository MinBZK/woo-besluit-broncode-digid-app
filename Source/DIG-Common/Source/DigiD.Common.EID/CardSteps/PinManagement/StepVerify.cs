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
﻿using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;

namespace DigiD.Common.EID.CardSteps.PinManagement
{
    internal class StepVerify : BaseSecureStep, IStep
    {
        private readonly CardCredentials _credentials;
        private readonly PasswordType _passwordType;

        public StepVerify(ISecureCardOperation operation, CardCredentials credentials, PasswordType passwordType) : base(operation)
        {
            _credentials = credentials;
            _passwordType = passwordType;
        }

        public async Task<bool> Execute()
        {
            var command = CommandApduBuilder.GetVerifyCommand(_credentials.Password(_passwordType), _passwordType, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("Verify", command, Operation.GAP.SMContext);

            switch (response.SW)
            {
                case 0x9000:
                    return true;
                case 0x63c0:
                    Operation.GAP.IsBlocked = true;
                    Operation.GAP.AuthenticationResult = AuthenticationResult.Failed;
                    break;
            }

            return false;
        }
    }
}
