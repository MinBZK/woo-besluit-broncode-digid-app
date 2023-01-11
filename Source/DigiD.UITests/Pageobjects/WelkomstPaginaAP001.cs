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
using Belastingdienst.MCC.TestAAP.Commons;

namespace DigiD.UITests.Pageobjects
{
    public class WelkomstPaginaAP001 : Pageobject<WelkomstPaginaAP001>
    {
        private const string _title = "Welkom";
        private const string _waitText = "De makkelijkste manier om veilig in te loggen";
        public const string StatusWelkomstPagina = "De app moet nog geactiveerd worden.";
        private const string _startButton = "Start";
        
        private WelkomstPaginaAP001(string title) : base(title, _waitText) { }

        

        public static WelkomstPaginaAP001 Load(string title = _title)
            => new WelkomstPaginaAP001(title);

        public WelkomstPaginaAP001 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);


        public WelkomstPaginaAP001 ControleerStatusApp()
            => WaitForElementToAppear(StatusWelkomstPagina);

        public WelkomstPaginaAP001 StartActivatie()
            => TapOn(_startButton);



    }
}
