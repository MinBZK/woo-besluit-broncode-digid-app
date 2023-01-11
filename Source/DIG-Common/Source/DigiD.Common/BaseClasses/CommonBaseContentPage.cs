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
