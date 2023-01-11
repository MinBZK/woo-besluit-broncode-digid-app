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
    public class IDCheckUitstellenPagina_AP062 : Pageobject<IDCheckUitstellenPagina_AP062>
    {
        private const string _title = "ID-check uitstellen";
        //private const string _waitText = "U heeft ervoor gekozen om de ID-check niet nu uit te voeren. Wilt u dat wij u er op een later moment aan herinneren dit alsnog te doen?";
        static Xquery _waitText = new Xquery()
        {
            Android = c => c.Marked("U heeft ervoor gekozen om de ID-check niet nu uit te voeren. Wilt u dat wij u er op een later moment aan herinneren dit alsnog te doen?"),
            iOS = c => c.Text("U heeft ervoor gekozen om de ID-check niet nu uit te voeren. Wilt u dat wij u er op een later moment aan herinneren dit alsnog te doen?")
        };
        private const string _neeButton = "Nee";
        private const string _jaButton = "Ja";

        private IDCheckUitstellenPagina_AP062(string title) : base(title, _waitText) { }

        public static IDCheckUitstellenPagina_AP062 Load(string title = _title)
            => new IDCheckUitstellenPagina_AP062(title);

        public IDCheckUitstellenPagina_AP062 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public IDCheckUitstellenPagina_AP062 ActiveerKnopJa()
           => TapOn(_jaButton);

        public IDCheckUitstellenPagina_AP062 ActiveerKnopNee()
           => TapOn(_neeButton);




    }
}
