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

namespace DigiD.UITests.Pageobjects
{
    public class WhatsNewPage3 : Pageobject<WhatsNewPage3>
    {
        private const string _title = "Toegankelijkheid";
        private const string _waitText = "Toegankelijkheid";
        public const string _WhatsNewTekst = "We willen dat iedereen de DigiD app kan gebruiken. Daarom hebben we de toegankelijkheid verbeterd.";
        private const string _volgendeButton = "Volgende";
        private const string _vorigeButton = "Vorige";
        private const string _sluitenButton = "Sluiten";


        private WhatsNewPage3(string title) : base(title, _waitText) { }

        public static WhatsNewPage3 Load(string title = _title)
            => new WhatsNewPage3(title);

        public WhatsNewPage3 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_WhatsNewTekst);

        public WhatsNewPage3 GaNaarVolgendePagina()
            => TapOn(_volgendeButton);

        public WhatsNewPage3 GaNaarVorigePagina()
            => TapOn(_vorigeButton);

        public WhatsNewPage3 SluitPagina()
           => TapOn(_sluitenButton);


    }
}
