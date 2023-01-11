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
    public class DonkereModusPagina_AP097 : Pageobject<DonkereModusPagina_AP097>
    {
        private const string _title = "Donkere modus";
        private const string _waitText = "Bij 'automatisch' volgt de app vanzelf de donkere modus-instelling van uw mobiele apparaat.";
        private const string _DonkereModusAanButton = "Aan";
        private const string _DonkereModusUitButton = "Uit";
        private const string _DonkereModusAutomatischButton = "Automatisch";
        private const string _sluitButton = "Huidige actie annuleren";


        private DonkereModusPagina_AP097(string title) : base(title, _waitText) { }

        public static DonkereModusPagina_AP097 Load(string title = _title)
            => new DonkereModusPagina_AP097(title);

        public DonkereModusPagina_AP097 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public DonkereModusPagina_AP097 DonkereModusAan()
             => TapOn(_DonkereModusAanButton);

        public DonkereModusPagina_AP097 DonkereModusUit()
             => TapOn(_DonkereModusUitButton);

        public DonkereModusPagina_AP097 DonkereModusAutomatisch()
             => TapOn(_DonkereModusAutomatischButton);

        public DonkereModusPagina_AP097 PaginaSluiten()
            => TapOn(_sluitButton);



    }
}
