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
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.CardSteps.Randomize
{
    /// <summary>
    /// The client checks that the APDU's from the previous step are in the correct order.
    /// The client sends step the secured APDU's to the PCA.
    /// See step 18, page 26 of the PCA implementation guidelines.
    /// </summary>
    internal class StepSendSecureCommandsToPca : BaseSecureStep, IStep
    {
        public StepSendSecureCommandsToPca(ISecureCardOperation operation) : base(operation)
        {
        }

        public async Task<bool> Execute()
        {
            var card = (DrivingLicense) Operation.GAP.Card;

            var index = -1;

            //Clear al historical responses from cache
            card.ResponseAPDUs.Clear();

            foreach (var apdu in card.CommandAPDUs)
            {
                var bytes = Convert.FromBase64String(apdu);
                index++;

                Console.WriteLine($"Index: {index}: {apdu}");

                // Validate the APDU's we've gotten from the server.
                if (!CommandApduBuilder.ValidateAPDU(bytes, index)) continue;

                var result = await EIDSession.NfcService.SendApduAsync(bytes);
                var request = new CommandApdu(bytes);
                Debugger.DumpInfo("SERVER COMMAND", request);

                if (!result.Success) continue;

                var responseApdu = new ResponseApdu(result.Data);
                Debugger.DumpInfo("SERVER RESPONSE", responseApdu);
                
                if (responseApdu.SW == 0x9000)
                    card.ResponseAPDUs.Add(responseApdu.Bytes);
                else
                {
                    Debugger.WriteLine($"Received an incorrect response from PCA: {responseApdu.SW}");
                }
            }

            return true;
        }
    }
}
