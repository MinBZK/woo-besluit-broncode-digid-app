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
    public class UwEmailAdresPagina_AP114 : Pageobject<UwEmailAdresPagina_AP114>
    {
        private const string _title = "Uw e-mailadres";
        private const string _waitText = "Uw e-mailadres";
        private Query _informatie = x => x.Raw("* {text CONTAINS 'Het e-mailadres dat wij van u geregistreerd hebben is:'}");
        private const string _neeButton = "Nee";
        private const string _jaButton = "Ja";




        private UwEmailAdresPagina_AP114(string title) : base(title, _waitText) { }

        public static UwEmailAdresPagina_AP114 Load(string title = _title)
            => new UwEmailAdresPagina_AP114(title);

        public UwEmailAdresPagina_AP114 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_informatie);


        public UwEmailAdresPagina_AP114 EmailAdresAkkoord()
           => TapOn(_jaButton);

        public UwEmailAdresPagina_AP114 EmailAdresNietAkkoord()
           => TapOn(_neeButton);

    }
}
