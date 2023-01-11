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

namespace DigiD.UITests.Pageobjects
{
    public class WhatsNewPage3 : Pageobject<WhatsNewPage3>
    {
        private const string _title = "Toegankelijkheid";
        private const string _waitText = "Toegankelijkheid";
        public const string _WhatsNewTekst = "We willen dat iedereen de DigiD app kan gebruiken. Daarom hebben we de toegankelijkheid verbeterd.";
        private const string _volgendeButton = "Volgende";
        private const string _vorigeButton = "Vorige";
        private const string _sluitenButton = "Sluiten";


        private WhatsNewPage3(string title) : base(title, _waitText) { }

        public static WhatsNewPage3 Load(string title = _title)
            => new WhatsNewPage3(title);

        public WhatsNewPage3 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_WhatsNewTekst);

        public WhatsNewPage3 GaNaarVolgendePagina()
            => TapOn(_volgendeButton);

        public WhatsNewPage3 GaNaarVorigePagina()
            => TapOn(_vorigeButton);

        public WhatsNewPage3 SluitPagina()
           => TapOn(_sluitenButton);


    }
}
