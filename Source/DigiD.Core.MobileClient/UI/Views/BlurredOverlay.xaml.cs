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
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BlurredOverlay : Grid
	{
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BlurredOverlay));

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            
            if (propertyName == nameof(IsVisible) && IsVisible)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    // Spinner moet vierkant zijn en de kleinste waarde gebruiken
                    spinnerView.WidthRequest = OrientationHelper.IsInLandscapeMode  
                        ? Math.Min(DisplayHelper.Height, 500) 
                        : Math.Min(DisplayHelper.Width, 500);
                    spinnerView.HeightRequest = spinnerView.WidthRequest;
                    spinnerView.Play();
                });
            }
        }
        
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public BlurredOverlay ()
		{
			InitializeComponent ();
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            if (Command?.CanExecute(null) == true)
                Command.Execute(null);
        }

        private void SpinnerView_OnOnFinish(object sender, EventArgs e)
        {
            if (Command?.CanExecute(null) == true)
                Command.Execute(null);
        }
    }
}
