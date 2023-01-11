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
    public class PincodeWijzigenPagina_AP073 : Pageobject<PincodeWijzigenPagina_AP073>
    {
        private const string _title = "Pincode wijzigen";
        //private const string _waitText = "U gaat nu de pincode wijzigen van uw DigiD app. Weet u uw huidige pincode nog?";
        static Xquery _waitText = new Xquery()
        {
            Android = c => c.Marked("U gaat nu de pincode wijzigen van uw DigiD app. Weet u uw huidige pincode nog?"),
            iOS = c => c.Text("U gaat nu de pincode wijzigen van uw DigiD app. Weet u uw huidige pincode nog?")
        };
        private const string _knopPincodeVergeten = "Pincode vergeten";
        private const string _knopPincodeInvoeren = "Pincode invoeren";
        private const string _sluitButton = "Huidige actie annuleren";


        private PincodeWijzigenPagina_AP073(string title) : base(title, _waitText) { }

        public static PincodeWijzigenPagina_AP073 Load(string title = _title)
            => new PincodeWijzigenPagina_AP073(title);

        public PincodeWijzigenPagina_AP073 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public PincodeWijzigenPagina_AP073 PincodeWijzigen()
             => TapOn(_knopPincodeInvoeren);

        public PincodeWijzigenPagina_AP073 PincodeVergeten()
             => TapOn(_knopPincodeVergeten);

        public PincodeWijzigenPagina_AP073 PaginaSluiten()
            => TapOn(_sluitButton);

     
    }
}

