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
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public class CustomSvgImage : SvgCachedImage
    {
        private double MaxWidth => Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation == Xamarin.Essentials.DisplayOrientation.Landscape ? DisplayHelper.Height - 40 : DisplayHelper.Width - 40;

        public CustomSvgImage()
        {
            FadeAnimationEnabled = false;
            Aspect = Aspect.AspectFit;
            CacheType = FFImageLoading.Cache.CacheType.All;
            SizeChanged -= CustomSvgImage_SizeChanged;
            SizeChanged += CustomSvgImage_SizeChanged;
        }

        private void CustomSvgImage_SizeChanged(object sender, System.EventArgs e)
        {
            if (Width == 0 && WidthRequest == -1)
                WidthRequest = MaxWidth;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (widthConstraint > MaxWidth)
                WidthRequest = MaxWidth;

            if (!double.IsInfinity(heightConstraint) && !double.IsNaN(heightConstraint))
                HeightRequest =  heightConstraint;

            if (widthConstraint == 0)
                WidthRequest = 40;

            if (heightConstraint == 0)
                HeightRequest = 40;

            return base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}
