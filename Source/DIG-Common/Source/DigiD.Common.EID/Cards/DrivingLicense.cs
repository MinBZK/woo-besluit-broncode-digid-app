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
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.EID.CardSteps.CAv2;
using DigiD.Common.EID.CardSteps.TA;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.Cards
{
    public class DrivingLicense : Card
    {
        internal Certificate ATCertificate { get; set; }
        internal TerminalAuthenticationRdw TA { get; set; }
        internal ChipAuthenticationRdw CA { get; set; }
        internal EFCardSecurity CardSecurity { get; set; }
        public string[] CommandAPDUs { get; set; }
        public List<byte[]> ResponseAPDUs { get; } = new List<byte[]>();
        
        internal override async Task<bool> ReadFiles(bool withPhoto, SMContext context)
        {
            
            EFCOM = await SelectAndReadFile.Execute<EFCOM>(new CardFile(P1.CHILD_EF, "00-1E".ConvertHexToBytes()), context);
            if (EFCOM == null)
                return false;

            EF_SOd = await SelectAndReadFile.Execute<EFSOd>(new CardFile(P1.CHILD_EF, "00-1D".ConvertHexToBytes()), context);
            if (EF_SOd == null)
                return false;

            if (DataGroups.ContainsKey(14))
            {
                return true;
            }

            File file = await SelectAndReadFile.Execute<DG14>(new CardFile(P1.CHILD_EF, "00-0E".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(14, file);

            file = await SelectAndReadFile.Execute<DG1>(new CardFile(P1.CHILD_EF, "00-01".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(1, file);

            file = await SelectAndReadFile.Execute<DG5>(new CardFile(P1.CHILD_EF, "00-05".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(5, file);
            
            file = await SelectAndReadFile.Execute<DG11>(new CardFile(P1.CHILD_EF, "00-0B".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(11, file);
            
            file = await SelectAndReadFile.Execute<DG12>(new CardFile(P1.CHILD_EF, "00-0C".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(12, file);
            
            file = await SelectAndReadFile.Execute<DG13>(new CardFile(P1.CHILD_EF, "00-0D".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(13, file);

            if (withPhoto)
            {
                file = await SelectAndReadFile.Execute<DG6>(new CardFile(P1.CHILD_EF, "00-06".ConvertHexToBytes()), context);
                if (file == null)
                    return false;
                DataGroups.Add(6, file);
            }

            return true;
        }
    }
}
