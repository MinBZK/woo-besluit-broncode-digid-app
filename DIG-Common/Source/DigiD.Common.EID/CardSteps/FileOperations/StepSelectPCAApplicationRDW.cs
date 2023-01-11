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
﻿using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Exceptions;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.FileOperations
{
    internal class StepSelectPcaApplicationRdw : BaseSecureStep, IStep
    {
        public StepSelectPcaApplicationRdw(ISecureCardOperation operation) : base(operation)
        {
        }

        public async Task<bool> Execute()
        {
            if (Operation.GAP == null)
                Operation.GAP = (Gap)Operation;

            var command = CommandApduBuilder.GetSelectApplicationCommand(Operation.GAP.Card.PolymorphicAID, Operation.GAP.SMContext);
            var response = await CommandApduBuilder.SendAPDU("SelectPCAApplication", command, Operation.GAP.SMContext);

            switch (response.SW1)
            {
                case 0x90:
                    return true;
                case 0x69:
                    return true;
                case 0x63:
                    {
                        Operation.GAP.ChangePinRequired = true;
                        Operation.GAP.PinTriesLeft = (byte)response.SW2 & 0xf;

                        throw new TransportPinException();
                    }
                default:
                    return false;
            }
        }
    }
}
