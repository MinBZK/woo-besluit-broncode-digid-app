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
    public class ContactPagina_AP093 : Pageobject<ContactPagina_AP093>
    {
        private const string _title = "Contact";
        private const string _waitText = "Contact";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Heeft u een vraag over DigiD of deze app?'}");
        private const string _sluitButton = "Huidige actie annuleren";

        private ContactPagina_AP093(string title) : base(title, _waitText) { }

        public static ContactPagina_AP093 Load(string title = _title)
            => new ContactPagina_AP093(title);

        public ContactPagina_AP093 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public ContactPagina_AP093 VorigePagina()
            => Back();


    }

}
