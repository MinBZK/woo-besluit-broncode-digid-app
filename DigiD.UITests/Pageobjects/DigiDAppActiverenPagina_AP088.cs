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
    public class DigiDAppActiverenPagina_AP088 : Pageobject<DigiDAppActiverenPagina_AP088>
    {
        private const string _title = "DigiD app activeren";
        private const string _waitText = "De ID-check is niet gelukt. Wilt u het opnieuw proberen? U kunt ook uw DigiD app activeren met een sms-code. U moet dan later nog wel een keer uw identiteitsbewijs scannen. Dit is nodig om toegang te krijgen bij organisaties die erg privacygevoelige informatie van u hebben.";
        private const string _nogmaalsScannenButton = "Nogmaals scannen";
        private const string _activerenMetSMSButton = "Activeren met sms";


        private DigiDAppActiverenPagina_AP088(string title) : base(title, _waitText) { }

        public static DigiDAppActiverenPagina_AP088 Load(string title = _title)
            => new DigiDAppActiverenPagina_AP088(title);

        public DigiDAppActiverenPagina_AP088 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public DigiDAppActiverenPagina_AP088 ActiveerNogmaalsScannen()
            => TapOn(_nogmaalsScannenButton);

        public DigiDAppActiverenPagina_AP088 ActiveerActiverenMetSMS()
            => TapOn(_activerenMetSMSButton);

    }
}
