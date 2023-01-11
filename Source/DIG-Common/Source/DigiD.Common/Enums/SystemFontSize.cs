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
