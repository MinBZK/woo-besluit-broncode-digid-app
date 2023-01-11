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
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.CardSteps.FileOperations
{
    internal class StepReadDG14 : BaseSecureStep, IStep
    {
        public StepReadDG14(ISecureCardOperation operation) : base(operation)
        {

        }

        public async Task<bool> Execute()
        {
            Gap gap;

            if (Operation is Gap operation)
                gap = operation;
            else
                gap = Operation.GAP;

            if (gap.Card.DataGroups.ContainsKey(14))
                return true;

            var f = new CardFile(P1.CHILD_EF, (gap.Card.DocumentType == DocumentType.DrivingLicense ? "00-0E" : "01-0E").ConvertHexToBytes());
            var file = await SelectAndReadFile.Execute<DG14>(f, gap.SMContext);

            if (file == null)
                return false;

            gap.Card.DataGroups.Add(14, file);
            return true;
        }
    }
}
