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
    public class EmailAdresToevoegenPagina_AP101 : Pageobject<EmailAdresToevoegenPagina_AP101>
    {
        private const string _title = "Voeg uw e-mailadres toe";
        static Xquery _waitText = new Xquery()
        {
            //Android = c => c.Marked("U heeft nog geen e - mailadres toegevoegd aan uw DigiD.Met een e - mailadres kunnen wij contact met u opnemen.Bijvoorbeeld bij wijzigingen in uw DigiD.Wilt u nu uw e - mailadres toevoegen ? "),
            Android = c => c.Marked("U heeft nog geen e-mailadres toegevoegd aan uw DigiD. Met een e-mailadres kunnen wij contact met u opnemen. Bijvoorbeeld bij wijzigingen in uw DigiD. Wilt u nu uw e-mailadres toevoegen?"),
            iOS = c => c.Text("U heeft nog geen e-mailadres toegevoegd aan uw DigiD. Met een e-mailadres kunnen wij contact met u opnemen. Bijvoorbeeld bij wijzigingen in uw DigiD. Wilt u nu uw e-mailadres toevoegen?")
        };
        private const string _jaButton = "Ja";
        private const string _neeButton = "Nee";


        private EmailAdresToevoegenPagina_AP101(string title) : base(title, _waitText) { }

        public static EmailAdresToevoegenPagina_AP101 Load(string title = _title)
            => new EmailAdresToevoegenPagina_AP101(title);

        public EmailAdresToevoegenPagina_AP101 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public EmailAdresToevoegenPagina_AP101 ActiveerKnopJa()
            => TapOn(_jaButton);

        public EmailAdresToevoegenPagina_AP101 ActiveerKnopNee()
            => TapOn(_neeButton);



    }



}
