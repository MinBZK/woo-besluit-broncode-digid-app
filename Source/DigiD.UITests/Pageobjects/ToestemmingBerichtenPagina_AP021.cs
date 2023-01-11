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
using System.Threading.Tasks;
using Belastingdienst.MCC.TestAAP.Extensions;
using Belastingdienst.MCC.TestAAP.Commons;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace DigiD.UITests.Pageobjects
{
    public class ToestemmingBerichtenPagina_AP021 : Pageobject<ToestemmingBerichtenPagina_AP021>
    {
        private const string _title = "Toestemming berichten";
        static Xquery _waitText = new Xquery()
        {
            Android = c => c.Marked("Wij kunnen u via de app berichten sturen als er iets wijzigt in uw DigiD. Wilt u deze meldingen ontvangen?"),
            iOS = c => c.Text("Wij kunnen u via de app berichten sturen als er iets wijzigt in uw DigiD. Wilt u deze meldingen ontvangen?")
        };
        private const string _jaButton = "Ja";
        private const string _NeeButton = "Nee";


        


        private ToestemmingBerichtenPagina_AP021(string title) : base(title, _waitText) { }

        public static ToestemmingBerichtenPagina_AP021 Load(string title = _title)
            => new ToestemmingBerichtenPagina_AP021(title);

        public ToestemmingBerichtenPagina_AP021 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public ToestemmingBerichtenPagina_AP021 ActiveerKnopNee()
            => TapOn(_NeeButton);

        public ToestemmingBerichtenPagina_AP021 ActiveerKnopJa()
            => TapOn(_jaButton);


    }



}
