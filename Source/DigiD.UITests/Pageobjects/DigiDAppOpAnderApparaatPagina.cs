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
    public class DigiDAppOpAnderApparaatPagina : Pageobject<DigiDAppOpAnderApparaatPagina>
    {
        private const string _title = "Gebruikt u de DigiD app al op een ander apparaat?";
        private const string _waitText = "Gebruikt u de DigiD app al op een ander apparaat?";
        private const string _jaButton = "Ja";
        private const string _NeeButton = "Nee";
        private const string _sluitButton = "Sluiten";

        private DigiDAppOpAnderApparaatPagina (string title) : base(title, _waitText) { }

        public static DigiDAppOpAnderApparaatPagina Load(string title = _title)
            => new DigiDAppOpAnderApparaatPagina (title);

        public DigiDAppOpAnderApparaatPagina ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_title);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopNee()
            => TapOn(_NeeButton);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopJa()
            => TapOn(_jaButton);

        public DigiDAppOpAnderApparaatPagina ActiveerKnopSluiten()
            => TapOn(_sluitButton);


    }
}
