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
﻿using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class DisplayHelper
    {
        public static double Width => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        public static double? WidthAsOptionalDouble => Width;

        public static double Height => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        public static double? HeightAsOptionalDouble => Height;
        
        public static string ScreenSize => $"{Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width}x{Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height}";

        public static Size Size => new Size(Width, Height);
    }
}
