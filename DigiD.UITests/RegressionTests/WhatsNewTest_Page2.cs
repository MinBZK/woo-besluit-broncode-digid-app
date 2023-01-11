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
﻿using Belastingdienst.MCC.TestAAP.Model;
using DigiD.UITests.Pageobjects;
using NUnit.Framework;
using Shouldly;
using Xamarin.UITest;

namespace DigiD.UITests.RegressionTests
{

    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class WhatsNewTest_Page2 : Ap2TestBase
    {

        public WhatsNewTest_Page2() : this(Platform.iOS) { }
        public WhatsNewTest_Page2(Platform platform) : this(platform, AppOrientation.Portrait) { }
        public WhatsNewTest_Page2(Platform platform, AppOrientation appOrientation) : base(platform, appOrientation) { }



        // Reuseable reference to the page that we are testing.
        WhatsNewPage2 _page;


        // Initialises the test by executing the following before each test.<para/>
        // Note: The underlying setup in any base classes does not need to be called explicitly,
        // the NUunit framework does this and works bottom up.
        [SetUp]
        public void InitTest()
        {
            //laad pagina 1
            WhatsNewPage1.Load()
               .GaNaarVolgendePagina();

            _page = WhatsNewPage2
                    .Load();
        }

        [Test]
        public void ControleerWeergaveWhatsNewPagina2()
        {
            //app.Repl();
            _page.ControleerOfJuisteTekstWordtGetoond();
            _page.GaNaarVolgendePagina();
        }


    }
}
