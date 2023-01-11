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
using System.Linq;
using Belastingdienst.MCC.TestAAP.Commons;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace DigiD.UITests.Pageobjects
{

    public class ControleerEmailadres_AP112 : Pageobject<ControleerEmailadres_AP112>

    {
        private const string _title = "Controleer uw e-mailadres";
        private const string _waitText = "Controleer uw e-mailadres";
        private  Query _informatie = x => x.Raw("* {text CONTAINS 'U heeft langere tijd uw e-mailadres niet aangepast via de app of in Mijn DigiD.'}"); 
        private const string _neeknop = "Nee";
        private const string _jaknop = "Ja";

        private ControleerEmailadres_AP112(string title) : base(title, _waitText) { }

        public static ControleerEmailadres_AP112 Load(string title = _title)
            => new ControleerEmailadres_AP112(title);

        public ControleerEmailadres_AP112 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public ControleerEmailadres_AP112 MailAdresBevestigen()
            => TapOn(_jaknop);

        public ControleerEmailadres_AP112 MailAdresNietBevestigen()
            => TapOn(_neeknop);





    }



}
