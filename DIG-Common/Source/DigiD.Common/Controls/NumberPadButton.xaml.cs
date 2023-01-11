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
