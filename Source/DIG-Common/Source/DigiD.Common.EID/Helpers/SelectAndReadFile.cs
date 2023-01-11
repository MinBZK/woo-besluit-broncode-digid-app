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
using System.IO;
using System.Threading.Tasks;
using DigiD.Common.EID.Models;
using File = DigiD.Common.EID.Models.CardFiles.File;

namespace DigiD.Common.EID.Helpers
{
    public static class SelectAndReadFile
    {
        public static async Task<T> Execute<T>(CardFile file, SMContext context = null) where T: File
        {
            var command = CommandApduBuilder.GetSelectFileCommand(file, context);
            var response = await CommandApduBuilder.SendAPDU($"Select file: {file.Identifier}", command, context);

            if (response.SW == 0x9000 && (context == null || !context.IsBAC || response.IsValidResponse(context)))
            {
                using (var s = new MemoryStream())
                {
                    using (var bw = new BinaryWriter(s))
                    {
                        var i = 1;

                        while (true)
                        {
                            command = CommandApduBuilder.ReadSelectedFileCommand((int) s.Length, context);
                            response = await CommandApduBuilder.SendAPDU($"Part {i} read", command, context, false);

                            switch (response.SW)
                            {
                                case 0x00:
                                    return null;
                                //decrypt data if needed
                                case 0x9000 when context != null:
                                    response = context.IsBAC ? response.DecryptDES(context) : response.DecryptAES(context);
                                    break;
                            }

                            if (response.SW == 0x9000)
                                bw.Write(response.Data);

                            //Determine if End of File is reached
                            var eof = response.Nr < (context != null ? 0xDF : 0xFF);

                            if (eof)
                            {
                                var bytes = s.ToArray();
                                var result = (T)Activator.CreateInstance(typeof(T), bytes);
                                    
                                if (result.Parse())
                                    return result;
                            }

                            i++;
                        }
                    }
                }
            }

            return null;
        }
    }
}
