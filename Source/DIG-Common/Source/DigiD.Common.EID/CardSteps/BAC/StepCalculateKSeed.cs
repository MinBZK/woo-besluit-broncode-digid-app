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
﻿using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.BAC
{
    public class StepCalculateKSeed : IStep
    {
        private readonly BacOperation _operation;
        
        public StepCalculateKSeed(BacOperation operation)
        {
            _operation = operation;
        }

        public Task<bool> Execute()
        {
            var mrzHash = CryptoDesHelper.ComputeSHA1Hash(_operation.Mrz.ToBytes());

            // return 16 most significant bytes of hash
            var kSeed = mrzHash.Take(16).ToArray();
            _operation.KSeed = kSeed;

            return Task.FromResult(kSeed.Length > 0);
        }
    }
}
