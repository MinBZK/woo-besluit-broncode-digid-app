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
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace DigiD.UITests.Pageobjects
{
    public class MenuPagina_AP036 : Pageobject<MenuPagina_AP036>
    {
        private const string _title = "DigiD logo";
        private const string _waitText = "ID-check";
        private const string _idCheckButton = "ID-check";
        private Query _mijnDigiDButton = x => x.Raw("* {text CONTAINS 'Mijn DigiD'}");
        private Query _instellingenButton = x => x.Raw("* {text CONTAINS 'Instellingen'}");
        private Query _hulpEnInfoButton = x => x.Raw("* {text CONTAINS 'Hulp & Info'}");
        private Query _berichtenButton = x => x.Raw("* {text CONTAINS 'Berichten'}");


        private MenuPagina_AP036(string title) : base(title, _waitText) { }


        public static MenuPagina_AP036 Load(string title = _title)
            => new MenuPagina_AP036(title);

        public static MenuPagina_AP036 LoadNaActivatie(string title = _waitText)
             => new MenuPagina_AP036(title);

        public MenuPagina_AP036()
           => WaitForElementToAppear(_waitText);

        public MenuPagina_AP036 StartIDCheck()
            => TapOn(_idCheckButton);

        public MenuPagina_AP036 OpenBerichtenPagina()
           => TapOn(_berichtenButton);

        public MenuPagina_AP036 OpenMijnDigiD()
           => TapOn(_mijnDigiDButton);

        public MenuPagina_AP036 OpenInstellingenPagina()
           => TapOn(_instellingenButton);

        public MenuPagina_AP036 OpenHulpEnInfoPagina()
           => TapOn(_hulpEnInfoButton);

    }
}

