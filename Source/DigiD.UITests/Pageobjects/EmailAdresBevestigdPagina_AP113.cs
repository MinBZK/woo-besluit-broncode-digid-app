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
    public class EmailAdresBevestigdPagina_AP113 : Pageobject<EmailAdresBevestigdPagina_AP113>
    {
        private const string _title = "E-mailadres bevestigd";
        //private const string _waitText = "Uw e-mailadres is nu bevestigd.";
        static Xquery _waitText = new Xquery()
        {
            Android = c => c.Marked("Uw e-mailadres is nu bevestigd."),
            iOS = c => c.Text("Uw e-mailadres is nu bevestigd.")
        };
        private const string _okButton = "OK";
      
        private EmailAdresBevestigdPagina_AP113(string title) : base(title, _waitText) { }

        public static EmailAdresBevestigdPagina_AP113 Load(string title = _title)
            => new EmailAdresBevestigdPagina_AP113(title);

        public EmailAdresBevestigdPagina_AP113 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public EmailAdresBevestigdPagina_AP113 ActiveerKnopOK()
             => TapOn(_okButton);

    }
}
