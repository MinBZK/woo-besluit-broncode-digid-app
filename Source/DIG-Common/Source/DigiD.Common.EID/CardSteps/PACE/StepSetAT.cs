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
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Exceptions;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Exceptions;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <summary>
    /// Send the MSE set AT command to the PCA.
    /// See BSI TR Worked example 3.1, page 15.
    /// </summary>
    internal class StepSetAT : IStep
    {
        private readonly Gap _gap;

        public StepSetAT(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            var certificate = _gap.Card.DocumentType == DocumentType.DrivingLicense
                ? ((DrivingLicense) _gap.Card).ATCertificate
                : null;

            var command = CommandApduBuilder.GetMSESetATCommand(_gap.Card.EF_CardAccess.PaceInfo, certificate, _gap.Pace.PasswordType, _gap.SMContext);

            var response = await CommandApduBuilder.SendAPDU("PACE Set AT", command, _gap.SMContext);

            switch (response.SW1)
            {
                case 0x90:
                    _gap.AuthenticationResult = AuthenticationResult.Success;
                    break;
                case 0x63:
                    _gap.PinTriesLeft = ((byte)response.SW2 & 0xf);
                    _gap.AuthenticationResult = AuthenticationResult.Failed;
                    break;
            }

            //If there is a SMContext, the tries left should be 1 for reset
            if (IsSuccessOrWarning() || (response.SW == 0x63c1 && _gap.SMContext != null))
            {
                return true;
            }

            if (response.SW == 0x63c0)
            {
                _gap.IsBlocked = true;
                throw new CardBlockedException();
            }

            if (response.SW == 0x63c1)
            {
                _gap.IsSuspended = true;
                throw new CardSuspendException();
            }

            throw new CardException("Unknown response from card Set AT Pace");

            bool IsSuccessOrWarning()
            {
                return response.SW1 == 0x90 || (response.SW1 == 0x63 && response.SW2 >= 0xc2 && response.SW2 <= 0xcf);
            }
        }
    }
}
