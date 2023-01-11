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
    public class PincodeGewijzigdPagina_AP075 : Pageobject<PincodeGewijzigdPagina_AP075>
    {
        private const string _title = "Pincode gewijzigd";
        //private const string _waitText = "De pincode van uw DigiD app is succesvol gewijzigd.";

        static Xquery _waitText = new Xquery()
        {
            Android = c => c.Marked("De pincode van uw DigiD app is succesvol gewijzigd."),
            iOS = c => c.Text("De pincode van uw DigiD app is succesvol gewijzigd.")
        };
        private const string _knopOK = "OK";
        
        private PincodeGewijzigdPagina_AP075(string title) : base(title, _waitText) { }

        public static PincodeGewijzigdPagina_AP075 Load(string title = _title)
            => new PincodeGewijzigdPagina_AP075(title);

        public PincodeGewijzigdPagina_AP075 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public PincodeGewijzigdPagina_AP075 PincodeGewijzigdAkkoord()
            => TapOn(_knopOK);



    }
}
