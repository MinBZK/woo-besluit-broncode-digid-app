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
using DigiD.Common.Enums;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public class PinTile : ContentView
    {
        private readonly DelegateWeakEventManager _clickedEventManager = new DelegateWeakEventManager();

        public event EventHandler Clicked
        {
            add => _clickedEventManager.AddEventHandler(value);
            remove => _clickedEventManager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(PinTile), string.Empty, propertyChanged: TextPropertyChanged);

        private static void TextPropertyChanged(BindableObject binding, object oldValue, object newValue)
        {
            ((PinTile)binding).SetText((string)newValue);
        }

        private readonly CustomSvgImage _pinster;
        private readonly CustomFontLabel _label;
        private readonly Image _background = new Image();
        private PinTileState _state;

        public void SetText(string value)
        {
            _label.Text = value;
            _label.SetValue(AutomationProperties.IsInAccessibleTreeProperty, false);

            if (_state == PinTileState.Active)
                SetPinsterVisibility(string.IsNullOrEmpty(value));
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetTextVisibility(!string.IsNullOrEmpty(value));
                SetValue(TextProperty, value);
            }
        }

        public PinTileState State
        {
            get => _state;
            set
            {
                _state = value;
                SetBackground();
            }
        }

        private void SetPinsterVisibility(bool isVisible)
        {
            if (_pinster != null)
                AutomationProperties.SetIsInAccessibleTree(_pinster, false);

            _pinster?.FadeTo(isVisible ? 1 : 0, 200);
            _pinster?.ScaleTo(isVisible ? 0.8 : 0.5, 200);
        }

        private void SetTextVisibility(bool isVisible)
        {
            _label.Opacity = isVisible ? 0 : 1;
            _label.FadeTo(isVisible ? 1 : 0, 200);
        }

        private void SetBackground()
        {
            AutomationProperties.SetIsInAccessibleTree(_background, false);

            switch (_state)
            {
                case PinTileState.Active:
                    _background.Source = "pincode_on.png";

                    break;
                case PinTileState.Current:
                    _background.Source = "pincode_invoer_active.png";
                    TranslationY = -5;
                    this.TranslateTo(0, 0);
                    break;
                case PinTileState.Inactive:
                    _background.Source = "pincode_off.png";
                    break;
            }

            SetPinsterVisibility(_state == PinTileState.Active);
        }

        public PinTile()
        {
            State = PinTileState.Inactive;

            var outerView = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };

            outerView.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    _clickedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(Clicked));
                }),
                NumberOfTapsRequired = 1
            });

            var innerView = new Grid
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 0,
                RowSpacing = 0
            };

            _label = new CustomFontLabel
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = (Color)Application.Current.Resources["PinTileTextColor"],
                FontFamily = "RO-Bold",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            innerView.Children.Add(_label);
            outerView.Children.Add(_background);
            outerView.Children.Add(innerView);

            AutomationProperties.SetIsInAccessibleTree(_label, false);
            AutomationProperties.SetIsInAccessibleTree(_background, false);

            AutomationProperties.SetIsInAccessibleTree(innerView, false);
            AutomationProperties.SetIsInAccessibleTree(outerView, false);

            Content = outerView;
        }

        public PinTile(int numberOfTiles, int index, bool hidePinster = false)
        {
            var outerView = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };

            outerView.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    _clickedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(Clicked));
                })
            });

            var innerView = new Grid
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 0,
                RowSpacing = 0
            };

            _pinster = new CustomSvgImage
            {
                Source = numberOfTiles == 5 || Device.Idiom == TargetIdiom.Phone ? "resource://DigiD.Common.Resources.icon_pinster.svg?assembly=DigiD.Common" : "resource://DigiD.Common.Resources.icon_pinster_puk.svg?assembly=DigiD.Common",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = 5,
                Opacity = 0,
                Scale = 0.5,
                WidthRequest = (numberOfTiles == 5 || Device.Idiom == TargetIdiom.Phone) ? 50 : 30,
                HeightRequest = (numberOfTiles == 5 || Device.Idiom == TargetIdiom.Phone) ? 50 : 30,
                IsVisible = !hidePinster,
            };

            _label = new CustomFontLabel
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = (Color)Application.Current.Resources["PinTileTextColor"],
                FontFamily = "RO-Bold",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            State = index == 0 ? PinTileState.Current : PinTileState.Inactive;

            AutomationProperties.SetIsInAccessibleTree(_label, false);
            AutomationProperties.SetIsInAccessibleTree(_pinster, false);
            AutomationProperties.SetIsInAccessibleTree(_background, false);

            this.SetBinding(TextProperty, $"Pin{index + 1}Text");

            innerView.Children.Add(_label);
            innerView.Children.Add(_pinster);
            outerView.Children.Add(_background);
            outerView.Children.Add(innerView);

            AutomationProperties.SetIsInAccessibleTree(innerView, false);
            AutomationProperties.SetIsInAccessibleTree(outerView, false);
            Content = outerView;
        }
    }
}
