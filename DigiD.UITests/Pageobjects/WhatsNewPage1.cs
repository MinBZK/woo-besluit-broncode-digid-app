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
using Belastingdienst.MCC.TestAAP.Commons;
using Xamarin.UITest;

namespace DigiD.UITests.Pageobjects
{
    public class WhatsNewPage1 : Pageobject<WhatsNewPage1>
    {
        private const string _title = "Gebruiksgeschiedenis";
        private const string _waitText = "Gebruiksgeschiedenis";
        public const string _WhatsNewTekst = "Bekijk het gebruik van uw DigiD in de app. U vindt dit in het menu onder 'Mijn Digid'.";
        private const string _volgendeButton = "Volgende";
        private const string _vorigeButton = "Vorige";
        private const string _sluitenButton = "Sluiten";

        private WhatsNewPage1(string title) : base(title, _waitText) { }

        public WhatsNewPage1(IApp app)
        {
        }

        public static WhatsNewPage1 Load(string title = _title)
            => new WhatsNewPage1(title);

        public WhatsNewPage1 ControleerOfJuisteTekstWordtGetoond()
            => WaitForTextToDisappear(_WhatsNewTekst);

        public WhatsNewPage1 GaNaarVolgendePagina()
            => TapOn(_volgendeButton);

        public WhatsNewPage1 GaNaarVorigePagina()
            => TapOn(_vorigeButton);

        public WhatsNewPage1 SluitPagina()
           => TapOn(_sluitenButton);


    }
}
