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
    public class MijnDigiDPagina_AP106 : Pageobject<MijnDigiDPagina_AP106>
    {
        private const string _title = "Mijn DigiD";
        private const string _waitText = "Overige gegevens";
        //private const string _emailadresButton = "E-mailadres";
        private Query _emailadresButton = x => x.Raw("* {text CONTAINS 'E-mailadres'}");
        private const string _naarMijnDigiDButton = "Naar Mijn DigiD";
        private Query _gebruiksgeschiedenisButton = x => x.Raw("* {text CONTAINS 'Gebruiksgeschiedenis'}");
        private Query _pincodeWijzigenButton = x => x.Raw("* {text CONTAINS 'Pincode wijzigen'}");




        private MijnDigiDPagina_AP106(string title) : base(title, _waitText) { }

        public static MijnDigiDPagina_AP106 Load(string title = _title)
            => new MijnDigiDPagina_AP106(title);

        public MijnDigiDPagina_AP106 EmailAdresToevoegen()
           => TapOn(_emailadresButton);

        public MijnDigiDPagina_AP106 GebruiksgeschiedenisOpenen()
           => TapOn(_gebruiksgeschiedenisButton);

        public MijnDigiDPagina_AP106 OpenPincodeWijzigen()
            => TapOn(_pincodeWijzigenButton);



    }
}
