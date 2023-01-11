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
    public class DeactiverenPagina_AP010 : Pageobject<DeactiverenPagina_AP010>
    {
        private const string _title = "DigiD app deactiveren";
        private const string _waitText = "DigiD app deactiveren";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Let op! Als u de DigiD app deactiveert, kunt u er niet meer mee inloggen.'}");
        private const string _neeButton = "Nee";
        private const string _jaButton = "Ja";
        private const string _sluitButton = "Huidige actie annuleren";
        private Query _informatiePopUp = x => x.Raw("* {text CONTAINS 'U gaat nu de DigiD app deactiveren'}");
        private const string _bevestigenButton = "Bevestigen";
        private const string _annulerenButton = "Bevestigen";

        private DeactiverenPagina_AP010(string title) : base(title, _waitText) { }

        public static DeactiverenPagina_AP010 Load(string title = _title)
            => new DeactiverenPagina_AP010(title);

        public DeactiverenPagina_AP010 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);

        public DeactiverenPagina_AP010 DeActiverenBevestigen()
            => TapOn(_jaButton);

        public DeactiverenPagina_AP010 DeActiverenAnnuleren()
            => TapOn(_neeButton);

        public DeactiverenPagina_AP010 VorigePagina()
            => Back();

        public DeactiverenPagina_AP010 DeActiveren()
            => TapOn(_bevestigenButton);

        public DeactiverenPagina_AP010 NietDeActiveren()
            => TapOn(_annulerenButton);

        public DeactiverenPagina_AP010 ControleerOfJuisteTekstWordtGetoondInPopUp()
            => WaitForElementToAppear(_informatiePopUp);


    }

}
