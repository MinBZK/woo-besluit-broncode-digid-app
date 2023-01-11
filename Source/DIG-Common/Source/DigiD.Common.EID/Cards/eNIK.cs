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
