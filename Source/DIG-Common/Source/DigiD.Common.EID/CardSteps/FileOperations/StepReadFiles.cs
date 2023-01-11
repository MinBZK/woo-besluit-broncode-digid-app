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
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.FileOperations
{
    internal class StepReadFiles : BaseSecureStep, IStep
    {
        private readonly bool _readPhoto;

        internal StepReadFiles(ISecureCardOperation operation, bool readPhoto) : base(operation)
        {
            _readPhoto = readPhoto;
        }

        public async Task<bool> Execute()
        {
            Gap gap;
            if (Operation is Gap operation)
                gap = operation;
            else
                gap = Operation.GAP;

            return await gap.Card.ReadFiles(_readPhoto, gap.SMContext);
        }
    }
}
