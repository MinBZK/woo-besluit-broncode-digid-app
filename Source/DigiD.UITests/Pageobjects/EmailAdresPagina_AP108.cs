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
    public class EmailAdresPagina_AP108 : Pageobject<EmailAdresPagina_AP108>
    {
        private const string _title = "E-mailadres";
        private const string _waitText= "E-mailadres";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'U heeft dit e-mailadres nog niet bevestigd.'}");
        private const string _overslaanButton = "Overslaan";
        private const string _nieuweCodeSturenButton = "Nieuwe code sturen";
        private const string _helpButton = "Extra informatie E-mailadres";
        
        private EmailAdresPagina_AP108(string title) : base(title, _waitText) { }

        public static EmailAdresPagina_AP108 Load(string title = _title)
            => new EmailAdresPagina_AP108(title);

        public EmailAdresPagina_AP108 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public EmailAdresPagina_AP108 OpenenHelpPagina()
            => TapOn(_helpButton);

        public EmailAdresPagina_AP108 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_waitText);

        public EmailAdresPagina_AP108 VulControleCodeEmail()
              => EnterText("SSSSSSSSS");


        public EmailAdresPagina_AP108 ActiveerKnopOverslaan()
            => TapOn(_overslaanButton);

        public EmailAdresPagina_AP108 NieuweCodeSturen()
            => TapOn(_nieuweCodeSturenButton);

    }
}
