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
    public class DigiDAppActiverenPaginaLetter_AP007 : Pageobject<DigiDAppActiverenPaginaLetter_AP007>
    {
        private const string _title = "DigiD app activeren";
        private const string _waitText = "Uw DigiD app is geactiveerd!";
        private const string _neeDankJeButton = "Nee, dank je";
        private const string __jaDoorgaanButton = "Ja, doorgaan";
        private const string __begrepenButton = "Begrepen";
        private const string _helpButton = "Extra informatie ";
        private const string _titleHelpPagina = "ID-check";
        private const string _gelezenButton = "Gelezen";


        private DigiDAppActiverenPaginaLetter_AP007(string title) : base(title, _waitText) { }

        public static DigiDAppActiverenPaginaLetter_AP007 Load(string title = _title)
            => new DigiDAppActiverenPaginaLetter_AP007(title);

        public DigiDAppActiverenPaginaLetter_AP007 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_waitText);

        public DigiDAppActiverenPaginaLetter_AP007 OpenenHelpPagina()
            => TapOn(_helpButton);

        public DigiDAppActiverenPaginaLetter_AP007 SluitHelpPagina()
            => TapOn(_gelezenButton);

        public DigiDAppActiverenPaginaLetter_AP007 ControleerTekstHelpPagina()
            => WaitForElementToAppear(_titleHelpPagina);

        public DigiDAppActiverenPaginaLetter_AP007 ActiveerKnopNeeDankJe()
            => TapOn(_neeDankJeButton);

        public DigiDAppActiverenPaginaLetter_AP007 ActiveerKnopJaDoorgaan()
            => TapOn(__jaDoorgaanButton);

        public DigiDAppActiverenPaginaLetter_AP007 ActiveerKnopBegrepen()
            => TapOn(__begrepenButton);

    }
}
