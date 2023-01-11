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
    public class GeenDigiDPagina_AP046 : Pageobject<GeenDigiDPagina_AP046>
    {
        private const string _title = "Geen DigiD";
        private const string _waitText = "Geen DigiD";
        private const string _ikWoonInBuitenlandButton = "Ik woon buiten Nederland";
        private const string _DigiDAanvragenButton = "DigiD aanvragen";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Als u geen DigiD heeft, kunt u direct in de app uw DigiD aanvragen.'}");


        private GeenDigiDPagina_AP046(string title) : base(title, _waitText) { }

        public static GeenDigiDPagina_AP046 Load(string title = _title)
            => new GeenDigiDPagina_AP046(title);

        public GeenDigiDPagina_AP046 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_informatie);

        public GeenDigiDPagina_AP046 ActiveerKnopBuitenland()
           => TapOn(_ikWoonInBuitenlandButton);

        public GeenDigiDPagina_AP046 ActiveerKnopDigiDAanvragen()
           => TapOn(_DigiDAanvragenButton);

    }
}
