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
﻿using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuSwitchControl : Grid
    {
        public static double? MenuSwitchNormalHeight = 56;
        public static double? MenuSwitchExtraLargeHeight = 112;

        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(MenuSwitchControl), false, BindingMode.TwoWay, propertyChanged: CheckedPropertyChanged);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MenuSwitchControl), null, propertyChanged: TextPropertyChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MenuSwitchControl));
        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(MenuSwitchControl), propertyChanged:ImageSourcePropertyChanged);

        private static void ImageSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((MenuSwitchControl) bindable).Image.Source = (ImageSource) newvalue;
        }

        private static void CheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((MenuSwitchControl)bindable).ToggleSwitch.IsToggled = (bool)newvalue;
        }

        private static void TextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((MenuSwitchControl)bindable).Label.Text = (string)newvalue;
        }

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set => SetValue(CheckedProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public MenuSwitchControl()
        {
            InitializeComponent();
        }

        private bool _initialized;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _initialized = true;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (HeightRequestProperty.PropertyName == propertyName)
            {
                SetRowHeight(HeightRequest);
            }
        }

        public void SetRowHeight(GridLength height)
        {
            if (RowDefinitions.Count > 0)
                RowDefinitions[0].Height = height;
        }

        private void ToggleSwitch_OnToggled(object sender, ToggledEventArgs e)
        {
            Checked = e.Value;

            if (!_initialized)
                return;

            if (Command != null && Command.CanExecute(e.Value))
                Command.Execute(e.Value);
        }
    }
}
