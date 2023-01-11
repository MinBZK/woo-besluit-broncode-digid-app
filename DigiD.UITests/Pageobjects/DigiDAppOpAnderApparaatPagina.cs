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
    public class DigiDAppOpAnderApparaatPagina : Pageobject<DigiDAppOpAnderApparaatPagina>
    {
        private const string _title = "Gebruikt u de DigiD app al op een ander apparaat?";
        private const string _waitText = "Gebruikt u de DigiD app al op een ander apparaat?";
        private const string _jaButton = "Ja";
        private const string _NeeButton = "Nee";
        private const string _sluitButton = "Sluiten";

        private DigiDAppOpAnderApparaatPagina (string title) : base(title, _waitText) { }

        public static DigiDAppOpAnderApparaatPagina Load(string title = _title)
            => new DigiDAppOpAnderApparaatPagina (title);

        public DigiDAppOpAnderApparaatPagina ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_title);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopNee()
            => TapOn(_NeeButton);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopJa()
            => TapOn(_jaButton);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopSluiten()
            => TapOn(_sluitButton);


    }
}
