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
using System.Windows.Input;
using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomRadioButton : Grid
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(CustomRadioButton), string.Empty, BindingMode.OneWay, propertyChanged: TextPropertyChanged);

        private static void TextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((CustomRadioButton)bindable).SetText((string)newvalue);
        }

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool),
            typeof(CustomRadioButton), default(bool), BindingMode.TwoWay, propertyChanged: IsCheckedPropertyChanged);

        private static void IsCheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((CustomRadioButton)bindable).SetState((bool)newvalue);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CustomRadioButton), null, BindingMode.OneTime);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CustomRadioButton), null, BindingMode.OneTime);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public CustomRadioButton()
        {
            InitializeComponent();
            SetState(false);
        }

        public void SetState(bool newValue)
        {
            if (!newValue)
            {
                lbl.SetAppThemeColor(Label.TextColorProperty, (Color)Application.Current.Resources["TextColorLight"], (Color)Application.Current.Resources["TextColorDark"]);
                img.SetOnAppTheme<ImageSource>(CachedImage.SourceProperty, "resource://DigiD.Resources.digid_icon_radiobutton_app_off.svg", "resource://DigiD.Resources.digid_icon_radiobutton_app_off_dark.svg");
            }
            else
            {
                lbl.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");
                img.Source = "resource://DigiD.Resources.digid_icon_radiobutton_app_on.svg";
            }

            SetAutomationText(newValue);
        }

        private void SetAutomationText(bool newValue)
        {
            var suffix = newValue ? $", {AppResources.AccessibilitySelected}" : "";
            if (Device.RuntimePlatform == Device.iOS)
                AutomationProperties.SetName(lbl, $"{lbl.Text} {suffix}");
            else
                AutomationProperties.SetName(this, $"{lbl.Text} {suffix}");
        }

        public void SetText(string newText)
        {
            lbl.Text = newText;
            SetAutomationText(IsChecked.GetValueOrDefault(false));
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }
}
