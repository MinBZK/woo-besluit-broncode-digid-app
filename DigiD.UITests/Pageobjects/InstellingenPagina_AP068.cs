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
using System.Linq;
using Belastingdienst.MCC.TestAAP.Commons;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;


namespace DigiD.UITests.Pageobjects
{
    public class InstellingenPagina_AP068 : Pageobject<InstellingenPagina_AP068>
    {
        private const string _title = "Instellingen";
        private const string _waitText = "Mogen wij uw anonieme gegevens verzamelen, zodat wij deze app kunnen verbeteren?";
        private Query _taalButton = x => x.Raw("* {text CONTAINS 'Taal'}");
        private Query _donkereModusButton = x => x.Raw("* {text CONTAINS 'Donkere modus'}");
        
        private Query _deactiverenButton = x => x.Raw("* {text CONTAINS 'Deactiveren'}");
        private const string _sluitButton = "Huidige actie annuleren";


        private InstellingenPagina_AP068(string title) : base(title, _waitText) { }

        public static InstellingenPagina_AP068 Load(string title = _title)
            => new InstellingenPagina_AP068(title);

        public InstellingenPagina_AP068 ControleerOfJuisteTekstWordtGetoond()
            => WaitForElementToAppear(_waitText);

        public InstellingenPagina_AP068 OpenTaal()
             => TapOn(_taalButton);

        public InstellingenPagina_AP068 OpenDonkereModus()
             => TapOn(_donkereModusButton);

        public InstellingenPagina_AP068 PaginaSluiten()
             => TapOn(_sluitButton);



        public InstellingenPagina_AP068 Deactiveren()
            => TapOn(_deactiverenButton);

        
    }
}
