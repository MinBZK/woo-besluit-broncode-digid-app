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
    public class PincodeWijzigenPagina_AP095 : Pageobject<PincodeWijzigenPagina_AP095>
    {
        private const string _title = "Pincode wijzigen";
        private const string _waitText = "U kunt hier de pincode wijzigen van uw DigiD app of van uw identiteitsbewijs. Welke pincode wilt u wijzigen?";
        private const string _PincodeAppWijzigen = "Pincode van de app";
        private const string _PincodeIDBewijsWijzigen = "Pincode van het identiteitsbewijs";
        private const string _sluitButton = "Huidige actie annuleren";


        private PincodeWijzigenPagina_AP095(string title) : base(title, _waitText) { }

        public static PincodeWijzigenPagina_AP095 Load(string title = _title)
            => new PincodeWijzigenPagina_AP095(title);

        public PincodeWijzigenPagina_AP095 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public PincodeWijzigenPagina_AP095 PincodeAppWijzigen()
             => TapOn(_PincodeAppWijzigen);

        public PincodeWijzigenPagina_AP095 PincodeIDBewijsWijzigen()
             => TapOn(_PincodeIDBewijsWijzigen);

        public PincodeWijzigenPagina_AP095 PaginaSluiten()
            => TapOn(_sluitButton);



    }
}
