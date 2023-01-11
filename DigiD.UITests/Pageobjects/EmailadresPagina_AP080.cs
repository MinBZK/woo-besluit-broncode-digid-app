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
    public class EmailAdresPagina_AP080 : Pageobject<EmailAdresPagina_AP080>
    {
        private const string _title = "E-mailadres";
        private const string _waitText = "Vul het e-mailadres in waarop wij u kunnen bereiken. Er wordt een controlecode gestuurd naar dit adres.";
        private const string _overslaanButton = "Overslaan";
        private const string _volgendeButton = "Volgende";
        private const string _helpButton = "Extra informatie E-mailadres";
        private const string _titleHelpPagina = "Uw e-mailadres";
        private const string _gelezenButton = "Gelezen";
        private const string _JaButton = "Ja";
        private const string _NeeButton = "Nee";
        //private const string _invulveldEmailAdres = " Tooltip info";
        static Xquery _invulveldEmailAdres = new Xquery()
        {
            Android = c => c.Marked(" Tooltip info"),
            iOS = c => c.Id("E-mailadres")
        };
        private Query _geenMailadresPopUp = x => x.Raw("* {text CONTAINS 'Geen e-mailadres ingevuld'}");



        private EmailAdresPagina_AP080(string title) : base(title, _waitText) { }

        public static EmailAdresPagina_AP080 Load(string title = _title)
            => new EmailAdresPagina_AP080(title);

        public EmailAdresPagina_AP080 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public EmailAdresPagina_AP080 OpenenHelpPagina()
            => TapOn(_helpButton);

        public EmailAdresPagina_AP080 SluitHelpPagina()
            => TapOn(_gelezenButton);

        public EmailAdresPagina_AP080 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_titleHelpPagina);

        public EmailAdresPagina_AP080 VulMailAdres()
            => WaitForElementToAppear(_invulveldEmailAdres)
               .Tap(_invulveldEmailAdres)
               .EnterText("SSSSSSSSSSSSSS")
               .DismissKeyboard();
            

        public EmailAdresPagina_AP080 ActiveerKnopOverslaan()
            => TapOn(_overslaanButton);

        public EmailAdresPagina_AP080 ActiveerKnopVolgende()
            => TapOn(_volgendeButton);

        public EmailAdresPagina_AP080 WachtOpPopUp()
            => WaitForElementToAppear(_geenMailadresPopUp);

        public EmailAdresPagina_AP080 BevestigGeenMail()
           => TapOn(_JaButton);

        public EmailAdresPagina_AP080 AlsnogMail()
           => TapOn(_NeeButton);

    }



}
