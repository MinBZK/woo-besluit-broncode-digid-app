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
