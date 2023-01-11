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

namespace DigiD.Common.Controls
{
    public partial class NumberPadButton : ImageButton
    {
        private int _number = -1;

        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                CommandParameter = _number;

                if (_number <= 9)
                {
                    AutomationProperties.SetName(this, _number.ToString());
                    this.SetOnAppTheme<FileImageSource>(SourceProperty, $"pinbutton_{_number}.png", $"pinbutton_{_number}_dark.png");
                }
                else if (_number == 11)
                {
                    AutomationProperties.SetName(this, AppResources.Remove);
                    this.SetOnAppTheme<FileImageSource>(SourceProperty, "pinbutton_backspace.png", "pinbutton_backspace_dark.png");
                }
            }
        }

        public NumberPadButton()
        {
            InitializeComponent();
        }
    }
}
