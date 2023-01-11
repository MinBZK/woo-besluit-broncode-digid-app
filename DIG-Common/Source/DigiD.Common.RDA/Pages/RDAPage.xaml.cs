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
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.RDA.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.RDA.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RDAPage : BaseContentPage
    {
        public bool AnimationPortraitVisible { get; private set; }
        public bool AnimationLandscapeVisible { get; private set; }
        public bool ImagePortraitVisible { get; private set; }
        public bool ImageLandscapeVisible { get; private set; }

        double AnimationWidth => Device.Idiom == TargetIdiom.Tablet ? DisplayHelper.Width - OrientationHelper.TabletPaddingWidth : DisplayHelper.Width;
        double HorizontalPadding => (Device.Idiom == TargetIdiom.Tablet && OrientationHelper.IsInLandscapeMode) ? OrientationHelper.TabletPaddingWidth + 40 : 40;

        RdaViewModel _viewModel;

        public RDAPage()
        {
            InitializeComponent();
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait || Device.Idiom == TargetIdiom.Tablet)
            {
                AnimationPortraitVisible = true;
                animationViewPortrait.WidthRequest = AnimationWidth - HorizontalPadding; //ktr Device.Idiom == TargetIdiom.Phone ? DisplayHelper.Width - 40 : DisplayHelper.Width - OrientationHelper.TabletPaddingWidth;
                animationViewPortrait.HeightRequest = animationViewPortrait.WidthRequest;
            }
            else
            {
                AnimationLandscapeVisible = Device.Idiom == TargetIdiom.Phone ? true : false;
                animationViewLandscape.WidthRequest = DisplayHelper.Width / 2 - HorizontalPadding;
                animationViewLandscape.HeightRequest = animationViewLandscape.WidthRequest;
            }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _viewModel = BindingContext as RdaViewModel;
        }

        private void SetAnimationSize(DisplayOrientation orientation)
        {
            var width = AnimationWidth;

            var horizontalMargin = HorizontalPadding;

            if (orientation == DisplayOrientation.Landscape && Device.Idiom == TargetIdiom.Phone)
            {
                AnimationLandscapeVisible = _viewModel.AnimationVisible;
                AnimationPortraitVisible = false;
                ImagePortraitVisible = false;
                ImageLandscapeVisible = !_viewModel.AnimationVisible;

                animationViewPortrait.HeightRequest = 0;
                imageViewPortrait.HeightRequest = 0;

                if (AnimationLandscapeVisible)
                    animationViewLandscape.HeightRequest = width / 2 - horizontalMargin;
                if (ImageLandscapeVisible)
                    imageViewLandscape.HeightRequest = width / 2 - horizontalMargin;
            }
            else
            {
                AnimationPortraitVisible = _viewModel.AnimationVisible;
                AnimationLandscapeVisible = false;
                ImageLandscapeVisible = false;
                ImagePortraitVisible = !_viewModel.AnimationVisible;

                animationViewLandscape.HeightRequest = 0;
                imageViewLandscape.HeightRequest = 0;

                if (AnimationPortraitVisible)
                    animationViewPortrait.HeightRequest = width - horizontalMargin;
                if (ImagePortraitVisible)
                    imageViewPortrait.HeightRequest = width - horizontalMargin;
            }
            mainGrid.ForceLayout();
        }

        protected override void OrientationChanged(DisplayOrientation orientation)
        {
            base.OrientationChanged(orientation);
            SetAnimationSize(orientation);
        }
    }
}
