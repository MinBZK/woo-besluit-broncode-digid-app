// Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
// gericht is op transparantie en niet op hergebruik. Hergebruik van 
// de broncode is toegestaan onder de EUPL licentie, met uitzondering 
// van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
//
// This code has been disclosed in response to a request under the Dutch
// Open Government Act ("Wet open Overheid"). This implies that publication 
// is primarily driven by the need for transparence, not re-use.
// Re-use is permitted under the EUPL-license, with the exception 
// of source files that contain a different license.
//
// The archive that this file originates from can be found at:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// using the reference "Logius, publicly disclosed source code DigiD-App" 
//
// Other questions regarding this Open Goverment Act decision may be
// directed via email to open@logius.nl
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
