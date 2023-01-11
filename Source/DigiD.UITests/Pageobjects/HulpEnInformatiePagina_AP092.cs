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
    public class HulpEnInformatiePagina_AP092 : Pageobject<HulpEnInformatiePagina_AP092>
    {
        private const string _title = "Hulp en informatie";
        private const string _waitText = "Hulp en informatie";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Overig'}");
        private const string _sluitButton = "Huidige actie annuleren";
        private Query _veelGesteldeVragenButton = x => x.Raw("* {text CONTAINS 'Veelgestelde vragen'}");
        private Query _contactButton = x => x.Raw("* {text CONTAINS 'Contact'}");
        private Query _overDeDigiDAppButton = x => x.Raw("* {text CONTAINS 'Over de DigiD app'}");
        private Query _openSourceBibliothekenButton = x => x.Raw("* {text CONTAINS 'Open-source bibliotheken'}");
        


        private HulpEnInformatiePagina_AP092(string title) : base(title, _waitText) { }

        public static HulpEnInformatiePagina_AP092 Load(string title = _title)
            => new HulpEnInformatiePagina_AP092(title);

        public HulpEnInformatiePagina_AP092 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public HulpEnInformatiePagina_AP092 OpenVeelgesteldeVragen()
            => TapOn(_veelGesteldeVragenButton);

        public HulpEnInformatiePagina_AP092 OpenContact()
            => TapOn(_contactButton);

        public HulpEnInformatiePagina_AP092 OverDeDigiDApp()
            => TapOn(_overDeDigiDAppButton);

        public HulpEnInformatiePagina_AP092 OpenSourceBibliotheken()
            => TapOn(_openSourceBibliothekenButton);

        public HulpEnInformatiePagina_AP092 PaginaSluiten()
            => TapOn(_sluitButton);


    }

}
