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
    public class DigiDAppActiverenPagina_AP104 : Pageobject<DigiDAppActiverenPagina_AP104>
    {
        private const string _title = "DigiD app activeren";
        private const string _waitText = "DigiD app activeren";
        private const string _volgendeButton = "Volgende";
        private Query _idCheckNietGelukt = x => x.Raw("* {text CONTAINS 'De ID-check is niet gelukt. U gaat nu uw DigiD app activeren met een sms-code.'}");


        private DigiDAppActiverenPagina_AP104 (string title) : base(title, _waitText) { }

        public static DigiDAppActiverenPagina_AP104 Load(string title = _title)
            => new DigiDAppActiverenPagina_AP104(title);

        public DigiDAppActiverenPagina_AP104 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_idCheckNietGelukt);

        public DigiDAppActiverenPagina_AP104 ActiveerKnopVolgende()
           => TapOn(_volgendeButton);

    }
}
