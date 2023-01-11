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
