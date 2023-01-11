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
﻿namespace DigiD.Common.Enums
{
    public enum SystemFontSize
    {
        XS,
        S,
        M,
        L,
        XL,
        XXL,
        XXXL,
        ExtraM, // 200% voor Android
        ExtraL  // 200% voor iOS
    }

    // Voor iOS zijn het de waarden die je terugkrijgt via 'UIApplication.SharedApplication.PreferredContentSizeCategory'
    // Android werkt met fontscale en die verkrijg je middels 'Xamarin.Essentials.Platform.CurrentActivity.Resources.Configuration.FontScale'
    //  iOS     Android
    //  XS    -   0.8 => XS
    //  S     -   0.9 => S
    //  M     -   1.0 => M
    //  L     -   1.1 => L
    //  XL    -   1.3 => XL
    //  XXL   -   1.5 => XXL
    //  XXXL  -   1.7 => XXXL
    //  A11YM -   2.0 => ExtraM (200% voor Android)
    //  A11YL - n.v.t.=> ExtraL (200% voor iOS)

    // A11YM en A11YL is voluit UICTContentSizeCategoryAccessibilityM resp. UICTContentSizeCategoryAccessibilityL
}
