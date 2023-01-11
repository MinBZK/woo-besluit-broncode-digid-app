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
﻿ using System;
using Belastingdienst.MCC.TestAAP.Commons;

namespace DigiD.UITests.Pageobjects
{
    public class WelkomstPagina_AP001 : Pageobject<WelkomstPagina_AP001>
    {
        private const string _title = "Welkom";
        private const string _waitText = "De makkelijkste manier om veilig in te loggen";
        private const string _waitTextNaActivatie = "Ga naar een website en kies voor inloggen met de DigiD app.";
        public const string StatusWelkomstPagina = "De app moet nog geactiveerd worden.";
        private const string _StatusNaActivatie = "U heeft de app geactiveerd.";
        private const string _startButton = "Start";
        private const string _openMenuButton = "Open Menu pagina";

        private WelkomstPagina_AP001(string title) : base(title, _waitText) { }
        

        public static WelkomstPagina_AP001 Load(string title = _title)
            => new WelkomstPagina_AP001(title);

        public static WelkomstPagina_AP001 LoadNaActivatie(string title = _waitTextNaActivatie)
             => new WelkomstPagina_AP001(title);

        public WelkomstPagina_AP001 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);


        public WelkomstPagina_AP001 ControleerStatusApp()
            => WaitForElementToAppear(StatusWelkomstPagina);

        public WelkomstPagina_AP001 StartActivatie()
            => TapOn(_startButton);

        public WelkomstPagina_AP001 ControleerStatusNaActivatie()
            => WaitForElementToAppear(_StatusNaActivatie);

        public WelkomstPagina_AP001 OpenMenu()
            => TapOn(_openMenuButton);

    }
}
