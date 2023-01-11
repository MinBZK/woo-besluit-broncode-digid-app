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
