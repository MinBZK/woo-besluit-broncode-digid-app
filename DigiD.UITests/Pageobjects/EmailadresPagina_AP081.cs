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
    public class EmailAdresPagina_AP081 : Pageobject<EmailAdresPagina_AP081>
    {
        private const string _title = "E-mailadres";
        private const string _waitText = "Voer de controlecode in die is verstuurd naar demo1@digid.nl. Sluit de app niet af.";
        private const string _overslaanButton = "Overslaan";
        private const string _geenMailOntvangenButton = "Geen e-mail ontvangen";
        private const string _helpButton = "Extra informatie E-mailadres";
        private const string _titleHelpPagina = "E-mail bevestigen";
        private const string _gelezenButton = "Gelezen";


        private EmailAdresPagina_AP081(string title) : base(title, _waitText) { }

        public static EmailAdresPagina_AP081 Load(string title = _title)
            => new EmailAdresPagina_AP081(title);

        public EmailAdresPagina_AP081 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public EmailAdresPagina_AP081 OpenenHelpPagina()
            => TapOn(_helpButton);

        public EmailAdresPagina_AP081 SluitHelpPagina()
            => TapOn(_gelezenButton);

        public EmailAdresPagina_AP081 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_titleHelpPagina);

        public EmailAdresPagina_AP081 VulControleCodeEmail()
              => EnterText("SSSSSSSSS");


        public EmailAdresPagina_AP081 ActiveerKnopOverslaan()
            => TapOn(_overslaanButton);

        public EmailAdresPagina_AP081 ActiveerKnopGeenEmail()
            => TapOn(_geenMailOntvangenButton);

    }
}
