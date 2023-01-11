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
    public class WachtOpBriefStatusPagina_AP001 : Pageobject<WachtOpBriefStatusPagina_AP001>
    {
        private const string _title = "Wacht op brief";
        private const string _waitText = "Heeft u de brief ontvangen? Volg de aanwijzingen en gebruik de knop hieronder om de app te activeren.";
        private const string _geenBriefGehadButton = "Geen brief gehad";
        private const string _activerenButton = "Activeren";


        private WachtOpBriefStatusPagina_AP001(string title) : base(title, _waitText) { }

        public static WachtOpBriefStatusPagina_AP001 Load(string title = _title)
            => new WachtOpBriefStatusPagina_AP001(title);

        public WachtOpBriefStatusPagina_AP001 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public WachtOpBriefStatusPagina_AP001 ActiveerKnopActiveren()
            => TapOn(_activerenButton);

    }
}
