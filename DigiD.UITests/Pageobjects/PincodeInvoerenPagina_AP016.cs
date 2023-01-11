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
    public class PincodeInvoerenPagina_AP016 : Pageobject<PincodeInvoerenPagina_AP016>
    {
        private const string _title = "Pincode invoeren";
        private const string _titleBijWijzigen = "Pincode wijzigen";
        private const string _waitText = "Voer pincode in om in te loggen.";
        private const string _sluitenButton = "Sluiten";
        private const string _helpButton = "Extra informatie Pincode invoeren";
        private const string _titleHelpPagina = "Pincode invoeren";
        private const string _gelezenButton = "Gelezen";

        private PincodeInvoerenPagina_AP016(string title) : base(title, _waitText) { }

        public static PincodeInvoerenPagina_AP016 Load(string title = _title)
            => new PincodeInvoerenPagina_AP016(title);

        public PincodeInvoerenPagina_AP016 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public PincodeInvoerenPagina_AP016 OpenenHelpPagina()
            => TapOn(_helpButton);

        public PincodeInvoerenPagina_AP016 SluitHelpPagina()
            => TapOn(_gelezenButton);

        public PincodeInvoerenPagina_AP016 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_titleHelpPagina);

        public PincodeInvoerenPagina_AP016 EnterPin(string pincode) => EnterPin(pincode.ToCharArray().Select(c => c.ToString()).ToArray());

        public PincodeInvoerenPagina_AP016 EnterPin(string[] pincode)
        {
            foreach (string cijfer in pincode) TapOn(cijfer);
            return this;
        }

        public PincodeInvoerenPagina_AP016 GeldigePincode(string pincode = "SSSSS")
        {
            return WaitForElementToAppear(_title)
                .EnterPin(pincode);
                

        }

        public PincodeInvoerenPagina_AP016 PincodeWijzigen(string pincode = "SSSSS")
        {
            return WaitForElementToAppear(_titleBijWijzigen)
                .EnterPin(pincode);


        }


    }



}
