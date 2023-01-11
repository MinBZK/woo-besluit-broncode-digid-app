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
    public class DigiDAppActiverenPagina_AP087 : Pageobject<DigiDAppActiverenPagina_AP087>
    {
        private const string _title = "DigiD app activeren";
        private const string _waitText = "De ID-check is niet gelukt. Wilt u het opnieuw proberen? U kunt ook uw DigiD app activeren met een brief. U moet dan later nog wel een keer uw identiteitsbewijs scannen. Dit is nodig om toegang te krijgen bij organisaties die erg privacygevoelige informatie van u hebben.";
        private const string _nogmaalsScannenButton = "Nogmaals scannen";
        private const string _activerenMetBriefButton = "Activeren met brief";


        private DigiDAppActiverenPagina_AP087(string title) : base(title, _waitText) { }

        public static DigiDAppActiverenPagina_AP087 Load(string title = _title)
            => new DigiDAppActiverenPagina_AP087(title);

        public DigiDAppActiverenPagina_AP087 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public DigiDAppActiverenPagina_AP087 ActiveerNogmaalsScannen()
            => TapOn(_nogmaalsScannenButton);

        public DigiDAppActiverenPagina_AP087 ActiveerActiverenMetBrief()
            => TapOn(_activerenMetBriefButton);

        }
}
