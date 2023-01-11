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
    public class WelkomstPaginaNaActivatie_AP001 : Pageobject<WelkomstPaginaNaActivatie_AP001>
    {
        private const string _title = "Welkom";
        private const string _waitText = "Ga naar een website en kies voor inloggen met de DigiD app.";
        private const string _StatusNaActivatie = "U heeft de app geactiveerd.";
        private const string _startButton = "Start";
        //private const string _openMenuButton = "Open Menu pagina";
        static Xquery _openMenuButton = new Xquery()
        {
            Android = c => c.Marked("Menu"),
            iOS = c => c.Marked("icon_menu.png")
        };

        private WelkomstPaginaNaActivatie_AP001(string title) : base(title, _waitText) { }


        public static WelkomstPaginaNaActivatie_AP001 Load(string title = _title)
            => new WelkomstPaginaNaActivatie_AP001(title);

        public static WelkomstPaginaNaActivatie_AP001 LoadNaActivatie(string title = _waitText)
             => new WelkomstPaginaNaActivatie_AP001(title);

        public WelkomstPaginaNaActivatie_AP001 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);
                       
        public WelkomstPaginaNaActivatie_AP001 StartProces()
            => TapOn(_startButton);

        public WelkomstPaginaNaActivatie_AP001 ControleerStatusNaActivatie()
            => WaitForElementToAppear(_StatusNaActivatie);

        public WelkomstPaginaNaActivatie_AP001 OpenMenu()
            => TapOn(_openMenuButton);

    }
}
