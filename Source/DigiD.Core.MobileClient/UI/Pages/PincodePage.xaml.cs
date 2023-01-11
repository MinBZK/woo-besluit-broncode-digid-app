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
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PincodePage : BaseContentPage
    {
        public PincodePage()
        {
            DefaultElementName = "PinTiles";

            InitializeComponent();

            ChangeAppearance();

            SetGridOrientation();
        }

       protected override void OrientationChanged(DisplayOrientation orientation)
        {
            SetGridOrientation();
        }

        private void SetGridOrientation()
        {
            switch (DeviceDisplay.MainDisplayInfo.Orientation)
            {
                case DisplayOrientation.Landscape:
                    if (Device.Idiom == TargetIdiom.Phone)
                        SetLandscape();
                    else
                        SetPortrait();
                    break;
                case DisplayOrientation.Portrait:
                    SetPortrait();
                    break;
            }
        }

        private void SetPortrait()
        {
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            };

            Grid.SetColumn(NumPad, 0);
            Grid.SetColumn(scrollView, 0);
            Grid.SetColumn(LabelPincodeForgotten, 0);
            Grid.SetRow(NumPad, 0);
            Grid.SetRow(LabelPincodeForgotten, 1);
            Grid.SetRow(NumPad, 2);
        }

        private void SetLandscape()
        {
            var rightHanded = DeviceDisplay.MainDisplayInfo.Rotation == DisplayRotation.Rotation90;

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            grid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star }
                    };
            grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition {Height = GridLength.Star},
                new RowDefinition {Height = GridLength.Auto}
            };

            Grid.SetColumn(NumPad, rightHanded ? 1 : 0);
            Grid.SetColumn(scrollView, rightHanded ? 0 : 1);
            Grid.SetColumn(LabelPincodeForgotten, rightHanded ? 0 : 1);
            Grid.SetRow(LabelPincodeForgotten, 1);

            Grid.SetRow(NumPad, 0);
            Grid.SetRowSpan(NumPad, 2);
        }

        private void ViewModelOnPinErrorEvent(object o, EventArgs eventArgs)
        {
            ChangeAppearance();
            PinTiles.ForceLayout();
            PinTiles.Error();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ((PinCodeViewModel)BindingContext).PinErrorEvent += ViewModelOnPinErrorEvent;
            ChangeAppearance();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            NavigationPage.SetHasNavigationBar(this, !((PinCodeViewModel)BindingContext).PreventLock);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((PinCodeViewModel)BindingContext).PinErrorEvent -= ViewModelOnPinErrorEvent;
        }

        private void ChangeAppearance()
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                NumPad.Padding = new Thickness(1, 1, 1, 0);
            }
            else
            {
                if (Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Phone && DisplayHelper.Height >= 812)   // Phone X
                {
                    var margin = 20;
                    var topMargin = 35;

                    MeldingContainer.Margin = new Thickness(margin);
                    PinTiles.Margin = new Thickness(margin, topMargin, margin, margin);
                }
                else if (Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Phone && DisplayHelper.Height < 568)
                {    //iPhone 4s
                    MeldingContainer.Margin = new Thickness(5, 5, 5, -10);
                    PinTiles.Margin = new Thickness(5);
                }
                else
                {
                    MeldingContainer.Margin = new Thickness(20, 20, 20, -10);
                    PinTiles.Margin = new Thickness(20);
                }
            }
        }
    }
}
