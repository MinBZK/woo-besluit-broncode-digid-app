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
﻿using DigiD.Common.Helpers;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public partial class NumberPad : Grid
    {
        private const int maxHeightPortrait = 736;
        private const int maxHeightLandscape = 552;
        private const double factor = 0.4;

        private bool IsInLandscapeMode => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape;

        public NumberPad()
        {
            InitializeComponent();

            if (Device.Idiom == TargetIdiom.Desktop)
                HeightRequest = 400;
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                HeightRequest = IsInLandscapeMode ? maxHeightLandscape * .45 : maxHeightPortrait * factor;
                WidthRequest = DisplayHelper.Width * (IsInLandscapeMode ? factor : (factor + 0.1));
            }
            else
            {
                HeightRequest = DisplayHelper.Height * factor;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                HeightRequest = IsInLandscapeMode ? maxHeightLandscape * .45 : maxHeightPortrait * factor;
                WidthRequest = DisplayHelper.Width * (IsInLandscapeMode ? factor : (factor + 0.1));
            }
            base.OnSizeAllocated(width, height);
        }
    }
}
