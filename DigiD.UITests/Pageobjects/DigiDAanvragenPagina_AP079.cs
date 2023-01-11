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
    public class DigiDAanvragenPagina_AP079 : Pageobject<DigiDAanvragenPagina_AP079>
    {
        private const string _title = "DigiD aanvragen";
        private const string _waitText = "DigiD aanvragen";
        private const string _volgendeButton = "Volgende";
        private Query _invulveldBurgerServiceNummer = x => x.Marked("Bsn");
        private Query _invulveldGeboortedatum = x => x.Marked("DateOfBirth");
        private Query _invulveldPostcode = x => x.Marked("Postalcode").Class("UIView").Index(3);
        private Query _invulveldHuisnummer = x => x.Marked("HouseNumber");
        private Query _invulveldEnToevoeging = x => x.Marked("en toevoeging");



        private DigiDAanvragenPagina_AP079(string title) : base(title, _waitText) { }

        public static DigiDAanvragenPagina_AP079 Load(string title = _title)
            => new DigiDAanvragenPagina_AP079(title);

        public DigiDAanvragenPagina_AP079 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_title);

        public DigiDAanvragenPagina_AP079 ActiveerKnopVolgende()
           => TapOn(_volgendeButton);

        public DigiDAanvragenPagina_AP079 InvullenBSN(string bsn)
            => WaitForElementToAppear(_invulveldBurgerServiceNummer)
               .Tap(_invulveldBurgerServiceNummer)
                .EnterText(bsn)
                .DismissKeyboard();

        public DigiDAanvragenPagina_AP079 InvullenGebDatum(string geboortedatum)
            => WaitForElementToAppear(_invulveldGeboortedatum)
               .Tap(_invulveldGeboortedatum)
                .EnterText(geboortedatum);

        public DigiDAanvragenPagina_AP079 InvullenPostcode(string postcode)
            => WaitForElementToAppear(_invulveldPostcode)
               .Tap(_invulveldPostcode)
               .EnterText(postcode)
               .DismissKeyboard();

        public DigiDAanvragenPagina_AP079 InvullenHuisnummer(string huisnummer)
            => WaitForElementToAppear(_invulveldHuisnummer)
               .Tap(_invulveldHuisnummer)
                .EnterText(huisnummer)
                .DismissKeyboard();

        public DigiDAanvragenPagina_AP079 InvullenHuisnummerToevoeging(string huisnummer)
            => WaitForElementToAppear(_invulveldHuisnummer)
               .Tap(_invulveldHuisnummer)
                .EnterText(huisnummer)
                .DismissKeyboard();


    }
}

