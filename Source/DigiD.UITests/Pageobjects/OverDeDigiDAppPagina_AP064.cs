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
    public class OverDeDigiDAppPagina_AP064 : Pageobject<OverDeDigiDAppPagina_AP064>
    {
        private const string _title = "Over de DigiD app";
        private const string _waitText = "Over de DigiD app";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'De DigiD app is de makkelijkste manier om veilig in te loggen.'}");
        private const string _sluitButton = "Huidige actie annuleren";

        private OverDeDigiDAppPagina_AP064(string title) : base(title, _waitText) { }

        public static OverDeDigiDAppPagina_AP064 Load(string title = _title)
            => new OverDeDigiDAppPagina_AP064(title);

        public OverDeDigiDAppPagina_AP064 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public OverDeDigiDAppPagina_AP064 VorigePagina()
            => Back();


    }

}
