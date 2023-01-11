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
