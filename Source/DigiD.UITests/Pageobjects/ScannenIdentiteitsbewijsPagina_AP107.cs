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
    public class ScannenIdentiteitsbewijsPagina_AP107 : Pageobject<ScannenIdentiteitsbewijsPagina_AP107>
    {
        private const string _title = "Scannen identiteitsbewijs";
        private const string _waitText = "In de volgende stap scant u de chip in uw identiteitsbewijs met uw telefoon. Dit doet u door uw identiteitsbewijs tegen de achterkant van uw telefoon te houden.";
        private const string _volgendeButton = "Volgende";


        private ScannenIdentiteitsbewijsPagina_AP107(string title) : base(title, _waitText) { }

        public static ScannenIdentiteitsbewijsPagina_AP107 Load(string title = _title)
            => new ScannenIdentiteitsbewijsPagina_AP107(title);

        public ScannenIdentiteitsbewijsPagina_AP107 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public ScannenIdentiteitsbewijsPagina_AP107 ActiveerKnopVolgende()
            => TapOn(_volgendeButton);
    }

}
