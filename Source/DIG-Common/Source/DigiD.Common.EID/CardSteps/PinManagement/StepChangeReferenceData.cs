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
﻿using System.Security;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.PinManagement
{
    internal class StepChangeReferenceData : BaseSecureStep, IStep
    {
        private readonly SecureString _newPin;

        public StepChangeReferenceData(ISecureCardOperation operation, SecureString newPin) : base(operation)
        {
            _newPin = newPin;
        }

        public async Task<bool> Execute()
        {
            var command = CommandApduBuilder.GetChangeReferenceDataCommand(_newPin, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("Change Reference Data", command, Operation.GAP.SMContext);

            return response.SW == 0x9000;
        }
    }
}
