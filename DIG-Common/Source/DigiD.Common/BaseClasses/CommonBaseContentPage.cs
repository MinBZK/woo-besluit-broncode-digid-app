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
﻿using Xamarin.Forms;
using Xamarin.Essentials;

namespace DigiD.Common.BaseClasses
{
    public abstract class CommonBaseContentPage : ContentPage
    {
        /// <summary>
        /// Onderstaande property wordt gebruikt om voor accessibility de focus op
        /// eerste (default element) te zetten, zodat de blinde gebruiker direct bij
        /// de juiste actie is. De CustomContentPageRenderer bevat een functie, FindDefaultElement()
        /// die gebruikt onderstsaande property om de Accessibility "Cursor" goed te zetten.
        /// </summary>
        private string _defaultElementName;
        public string DefaultElementName
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultElementName))
                    _defaultElementName = "DefaultElement";
                return _defaultElementName;
            }
            set => _defaultElementName = value;
        }

        private void DisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            OrientationChanged(e.DisplayInfo.Orientation);
        }

        protected virtual void OrientationChanged(DisplayOrientation orientation) { }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DeviceDisplay.MainDisplayInfoChanged += DisplayInfoChanged;

            if (BindingContext is CommonBaseViewModel bc)
            {
                bc.OnAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceDisplay.MainDisplayInfoChanged -= DisplayInfoChanged;
        }

        protected override bool OnBackButtonPressed()
        {
            if (BindingContext is CommonBaseViewModel bc)
                return bc.OnBackButtonPressed();

            return true;
        }
    }
}
