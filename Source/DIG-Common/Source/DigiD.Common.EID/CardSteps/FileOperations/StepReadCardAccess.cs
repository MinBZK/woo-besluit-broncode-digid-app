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
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;

namespace DigiD.Common.EID.CardSteps.FileOperations
{
    public class StepReadCardAccess : IStep
    {
        private readonly Gap _gap;

        public StepReadCardAccess(Gap gap)
        {
            _gap = gap;
        }
        public async Task<bool> Execute()
        {
            var file = await SelectAndReadFile.Execute<EFCardAccess>(CardFile.EFCardAccess);

            if (file == null)
                return false;

            _gap.Card.EF_CardAccess = file;
            return true;
        }
    }
}
