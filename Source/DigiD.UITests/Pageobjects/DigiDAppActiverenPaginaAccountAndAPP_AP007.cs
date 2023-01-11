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
    public class DigiDAppActiverenPaginaAccountAndApp_AP007 : Pageobject<DigiDAppActiverenPaginaAccountAndApp_AP007>
    {
        private const string _title = "DigiD app activeren";
        private const string _waitText = "Uw DigiD account en DigiD app zijn geactiveerd!";
        private const string _neeDankJeButton = "Nee, dank je";
        private const string __jaDoorgaanButton = "Ja, doorgaan";
        private const string _helpButton = "Extra informatie ";
        private const string _titleHelpPagina = "ID-check";
        private const string _gelezenButton = "Gelezen";


        private DigiDAppActiverenPaginaAccountAndApp_AP007(string title) : base(title, _waitText) { }

        public static DigiDAppActiverenPaginaAccountAndApp_AP007 Load(string title = _title)
            => new DigiDAppActiverenPaginaAccountAndApp_AP007(title);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 OpenenHelpPagina()
            => TapOn(_helpButton);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 SluitHelpPagina()
            => TapOn(_gelezenButton);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_titleHelpPagina);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 ActiveerKnopNeeDankJe()
            => TapOn(_neeDankJeButton);

        public DigiDAppActiverenPaginaAccountAndApp_AP007 ActiveerKnopJaDoorgaan()
            => TapOn(__jaDoorgaanButton);

    }
}
