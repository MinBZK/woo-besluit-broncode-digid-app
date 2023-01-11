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
