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
using System.Collections.Generic;
using DigiD.Common.Models;

namespace DigiD.Common.Helpers
{
    public static class QRCodeHelper
    {
        public static QRScanResult ProcessScan(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            try
            {
                var result = new QRScanResult { Identifier = uri.Scheme };  

                var values = new Dictionary<string, string>();

                foreach (var kv in uri.PathAndQuery.Split('&'))
                {
                    var indexOfSplit = kv.IndexOf('=');
                    var key = kv.Substring(0, indexOfSplit);
                    var value = kv.Substring(indexOfSplit + 1);
                    values.Add(key, value);
                }

                if (!values.ContainsKey("app_session_id") && !values.ContainsKey("vPUK"))
                    return null;

                result.Properties = values;

                return result;
            }
            catch (Exception ex)
            {
                AppCenterHelper.TrackError(ex);
                return null;
            }
        }
    }
}
