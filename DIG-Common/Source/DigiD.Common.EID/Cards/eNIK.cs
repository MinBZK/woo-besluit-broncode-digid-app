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
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;
using Org.BouncyCastle.Crypto.Parameters;

namespace DigiD.Common.EID.Cards
{
    public class eNIK : Card
    {
        internal List<string> TACommands { get; set; }
        internal List<string> PCACommands { get; set; }
        internal ECPublicKeyParameters CardAuthenticationPublicKey { get; set; }
        internal EFCVCA CVCA { get; set; }

        internal override async Task<bool> ReadFiles(bool withPhoto, SMContext context)
        {
            EFCOM = await SelectAndReadFile.Execute<EFCOM>(new CardFile(P1.CHILD_EF, "01-1E".ConvertHexToBytes()), context);
            if (EFCOM == null)
                return false;

            EF_SOd = await SelectAndReadFile.Execute<EFSOd>(new CardFile(P1.CHILD_EF, "01-1D".ConvertHexToBytes()), context);
            if (EF_SOd == null)
                return false;

            File file = await SelectAndReadFile.Execute<DG1>(new CardFile(P1.CHILD_EF, "01-01".ConvertHexToBytes()), context);

            if (file == null)
                return false;
            DataGroups.Add(1, file);

            file = await SelectAndReadFile.Execute<DG15>(new CardFile(P1.CHILD_EF, "01-0F".ConvertHexToBytes()), context);
            if (file == null)
                return false;
            DataGroups.Add(15, file);

            if (withPhoto)
            {
                file = await SelectAndReadFile.Execute<DG2>(new CardFile(P1.CHILD_EF, "01-02".ConvertHexToBytes()), context);
                if (file == null)
                    return false;

                DataGroups.Add(2, file);
            }

            return true;
        }
    }
}
