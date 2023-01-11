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
    public class TaalPagina_AP096 : Pageobject<TaalPagina_AP096>
    {
        private const string _title = "Taal";
        private const string _waitText = "Taal";
        private const string _NederlandsButton = "Nederlands";
        private const string _EngelsButton = "English";
        private const string _titleEngels = "Language";
        private const string _sluitButton = "Huidige actie annuleren";


        private TaalPagina_AP096(string title) : base(title, _waitText) { }

        public static TaalPagina_AP096 Load(string title = _title)
            => new TaalPagina_AP096(title);

        public TaalPagina_AP096 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public TaalPagina_AP096 TaalInEngels()
             => TapOn(_EngelsButton);

        public TaalPagina_AP096 TaalInNederlands()
             => TapOn(_NederlandsButton);

        public TaalPagina_AP096 ControleerWijzigingTaalEngels()
            => WaitForElementToAppear(_titleEngels);

        public TaalPagina_AP096 ControleerWijzigingTaalNederlands()
            => WaitForElementToAppear(_title);

        public TaalPagina_AP096 PaginaSluiten()
            =>  TapOn(_sluitButton);



    }
}
