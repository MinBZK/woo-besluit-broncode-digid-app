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
    internal class StepReadEFCOM : BaseSecureStep, IStep
    {
        private readonly Gap _operation;

        internal StepReadEFCOM(Gap operation) : base(operation)
        {
            _operation = operation;
        }

        public async Task<bool> Execute()
        {
            if (_operation.Card.EFCOM != null)
                return true;

            var fileId = _operation.Card.DocumentType == DocumentType.DrivingLicense ? "00-1E" : "01-1E";
            var file = await SelectAndReadFile.Execute<EFCOM>(new CardFile(P1.CHILD_EF, fileId.ConvertHexToBytes()), _operation.SMContext);
            Debugger.DumpInfo("EFCOM", file.Bytes);
            
            _operation.Card.EFCOM = file;
            return true;
        }
    }
}
