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
﻿using DigiD.Common.Enums;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public partial class BorderedButton : Button
    {
        public static double? DefaultButtonHeight = 56;
        public static double? XLButtonHeight = 112;  // 2 * defaultHeight

        public static double DefaultButtonHeightValue => DefaultButtonHeight.Value;
        public static double XLButtonHeightValue => XLButtonHeight.Value;

        public static readonly BindableProperty ButtonTypeProperty = BindableProperty.Create(nameof(ButtonType), typeof(ButtonType),
            typeof(BorderedButton), ButtonType.Primary, BindingMode.TwoWay, propertyChanged: ButtonTypePropertyChanged);

        private static void ButtonTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null)
            {
                var bb = (BorderedButton)bindable;
                var newType = (ButtonType)newValue;
                bb.ButtonType = newType;
                bb.ChangeButtonState();
            }
        }

        public ButtonType ButtonType
        {
            get => (ButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public bool IsPrimaryButton => ButtonType == ButtonType.Primary;

        public BorderedButton()
        {
            InitializeComponent();
            ChangeButtonState();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            ChangeButtonState();
        }

        public void ChangeButtonState()
        {
            if (IsEnabled)
                VisualStateManager.GoToState(this, IsPrimaryButton ? ButtonType.Primary.ToString() : ButtonType.Secundairy.ToString());
            else
                VisualStateManager.GoToState(this, "Disabled");
        }

    }
}
