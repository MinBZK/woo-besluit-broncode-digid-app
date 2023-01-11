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
