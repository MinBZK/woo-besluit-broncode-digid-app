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
    public class OpenSourceBibliothekenPagina_AP085 : Pageobject<OpenSourceBibliothekenPagina_AP085>
    {
        private const string _title = "Open-source bibliotheken";
        private const string _waitText = "Open-source bibliotheken";
        private const string _naarVoorgaandePagina = "Vorige";

        private OpenSourceBibliothekenPagina_AP085(string title) : base(title, _waitText) { }

        public static OpenSourceBibliothekenPagina_AP085 Load(string title = _title)
            => new OpenSourceBibliothekenPagina_AP085(title);

        //public OpenSourceBibliothekenPagina_AP085 VorigePagina()
        //    => TapOn(_naarVoorgaandePagina);

        public OpenSourceBibliothekenPagina_AP085 VorigePagina()
        {

            if (OnAndroid)
            {
                Back(); 
            }
            if (OniOS)
            {
                TapOn(_naarVoorgaandePagina);
            }

            return this;





        }



    }

}

