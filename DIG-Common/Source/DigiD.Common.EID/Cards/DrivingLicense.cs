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
