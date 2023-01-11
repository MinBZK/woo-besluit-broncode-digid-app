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
﻿using System.Threading;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DigiD.Models
{
    public class WhatsNewPageModel : BindableObject
    {
        public string AlternateText { get; set; }
        public bool IsAnimation => Source.EndsWith(".json", true, Thread.CurrentThread.CurrentUICulture);
        public bool PlayAnimation { get; set; }
        public string Source { get; set; }

        private string _text;
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_text))
                    return Device.RuntimePlatform == Device.Android
                        ? AndroidSpecificText
                        : iOSSpecificText;

                return _text;
            }
            set => _text = value;
        }

        private string _title;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                    return Device.RuntimePlatform == Device.Android
                        ? AndroidSpecificTitle
                        : iOSSpecificTitle;

                return _title;
            }
            set => _title = value;
        }

        public string AndroidSpecificTitle { get; set; }
        public string iOSSpecificTitle { get; set; }
        public string AndroidSpecificText { get; set; }
        public string iOSSpecificText { get; set; }

        [JsonIgnore]
        public string AnimationSource => IsAnimation ? Source : null;

        [JsonIgnore]
        public string ImageSource => IsAnimation ? null : Source;
    }
}
