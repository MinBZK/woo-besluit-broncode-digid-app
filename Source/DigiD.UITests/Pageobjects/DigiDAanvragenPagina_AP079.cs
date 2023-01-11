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

