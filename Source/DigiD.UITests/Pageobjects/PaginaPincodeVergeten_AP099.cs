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
    public class PincodeVergetenPagina_AP099 : Pageobject<PincodeVergetenPagina_AP099>
    {
        private const string _title = "Pincode vergeten";
        private const string _waitText = "Pincode vergeten";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Het is helaas niet mogelijk om uw pincode te wijzigen als u deze vergeten bent.'}");
        private const string _knopOpnieuwActiveren = "Opnieuw activeren";
        private const string _sluitButton = "Huidige actie annuleren";

        private PincodeVergetenPagina_AP099(string title) : base(title, _waitText) { }

        public static PincodeVergetenPagina_AP099 Load(string title = _title)
            => new PincodeVergetenPagina_AP099(title);

        public PincodeVergetenPagina_AP099 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public PincodeVergetenPagina_AP099 OpnieuwActiveren()
            => TapOn(_knopOpnieuwActiveren);

        public PincodeVergetenPagina_AP099 PaginaSluiten()
            => TapOn(_sluitButton);

        public PincodeVergetenPagina_AP099 VorigePagina()
            => Back();



    }
}

