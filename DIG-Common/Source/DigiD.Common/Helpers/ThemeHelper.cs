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
﻿using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class ThemeHelper
    {
        public static bool IsAutomaticAppThemePossible => DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 10 
                                                          || DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 13 
                                                          || Device.Idiom == TargetIdiom.Desktop;

        public static bool IsInDarkMode => Application.Current?.RequestedTheme == OSAppTheme.Dark;

        public static string Convert(string imageSource, bool forceDark = false)
        {
            if (imageSource == null)
                throw new ArgumentNullException(nameof(imageSource));

            const string suffix = "_dark";

            if (IsInDarkMode || forceDark)
            {
                var svg = imageSource.Contains(".svg");

                // In darkmode, nu controleren of de filenaam eindigt op '_dark'
                // zo niet dan filenaam aanpassen.
                var f = imageSource.Split(new[] { ".svg",".png" }, StringSplitOptions.RemoveEmptyEntries);
                if (!f[0].EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                    imageSource = string.Concat(f[0], suffix, svg ? ".svg" : ".png", f.Length == 1 ? "" : f[1]);
            }
            else
            {
                var f = imageSource.Split(new[] { suffix }, StringSplitOptions.RemoveEmptyEntries);
                if (f.Length > 1)
                {
                    // filenaam bevat '_dark', deze verwijderen.
                    imageSource = string.Concat(f[0], f[1]);
                }
            }

            return imageSource;
        }
    }
}
