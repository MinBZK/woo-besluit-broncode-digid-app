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
using Belastingdienst.MCC.TestAAP.Commons;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace DigiD.UITests.Pageobjects
{
    public class MenuPagina_AP036 : Pageobject<MenuPagina_AP036>
    {
        private const string _title = "DigiD logo";
        private const string _waitText = "ID-check";
        private const string _idCheckButton = "ID-check";
        private Query _mijnDigiDButton = x => x.Raw("* {text CONTAINS 'Mijn DigiD'}");
        private Query _instellingenButton = x => x.Raw("* {text CONTAINS 'Instellingen'}");
        private Query _hulpEnInfoButton = x => x.Raw("* {text CONTAINS 'Hulp & Info'}");
        private Query _berichtenButton = x => x.Raw("* {text CONTAINS 'Berichten'}");


        private MenuPagina_AP036(string title) : base(title, _waitText) { }


        public static MenuPagina_AP036 Load(string title = _title)
            => new MenuPagina_AP036(title);

        public static MenuPagina_AP036 LoadNaActivatie(string title = _waitText)
             => new MenuPagina_AP036(title);

        public MenuPagina_AP036()
           => WaitForElementToAppear(_waitText);

        public MenuPagina_AP036 StartIDCheck()
            => TapOn(_idCheckButton);

        public MenuPagina_AP036 OpenBerichtenPagina()
           => TapOn(_berichtenButton);

        public MenuPagina_AP036 OpenMijnDigiD()
           => TapOn(_mijnDigiDButton);

        public MenuPagina_AP036 OpenInstellingenPagina()
           => TapOn(_instellingenButton);

        public MenuPagina_AP036 OpenHulpEnInfoPagina()
           => TapOn(_hulpEnInfoButton);

    }
}

