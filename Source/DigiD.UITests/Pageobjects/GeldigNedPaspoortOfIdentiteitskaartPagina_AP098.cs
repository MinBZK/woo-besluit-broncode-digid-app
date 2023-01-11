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
    public class GeldigNedPaspoortOfIdentiteitskaartPagina_AP098 : Pageobject<GeldigNedPaspoortOfIdentiteitskaartPagina_AP098>
    {
        private const string _title = "Heeft u een geldig Nederlands paspoort of identiteitskaart?";
        private const string _waitText = "Heeft u een geldig Nederlands paspoort of identiteitskaart?";
        private const string _jaButton = "Ja";
        private const string _neeButton = "Nee";


        private GeldigNedPaspoortOfIdentiteitskaartPagina_AP098(string title) : base(title, _waitText) { }

        public static GeldigNedPaspoortOfIdentiteitskaartPagina_AP098 Load(string title = _title)
            => new GeldigNedPaspoortOfIdentiteitskaartPagina_AP098(title);

        public GeldigNedPaspoortOfIdentiteitskaartPagina_AP098 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public GeldigNedPaspoortOfIdentiteitskaartPagina_AP098 ActiveerKnopJa()
            => TapOn(_jaButton);

        public GeldigNedPaspoortOfIdentiteitskaartPagina_AP098 ActiveerKnopNee()
            => TapOn(_neeButton);



    }



}
