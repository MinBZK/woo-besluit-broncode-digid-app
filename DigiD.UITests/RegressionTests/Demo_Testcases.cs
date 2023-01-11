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
using Belastingdienst.MCC.TestAAP.Commons;
using DigiD.UITests.Pageobjects;
using NUnit.Framework;
using Shouldly;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace DigiD.UITests.RegressionTests
{

    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Demo_Testcases : Ap2TestBase
    {

        public Demo_Testcases() : this(Platform.iOS) { }
        public Demo_Testcases(Platform platform) : this(platform, AppOrientation.Portrait) { }
        public Demo_Testcases(Platform platform, AppOrientation appOrientation) : base(platform, appOrientation) { }

        // Reuseable reference to the page that we are testing.




        // Initialises the test by executing the following before each test.<para/>
        // Note: The underlying setup in any base classes does not need to be called explicitly,
        // the NUunit framework does this and works bottom up.
        [SetUp]
        public void Setup()


        {

            WhatsNewPage1.Load()
               .GaNaarVolgendePagina();

            WhatsNewPage2.Load()
                .GaNaarVolgendePagina();

            WhatsNewPage3.Load()
                .SluitPagina();


        }

        //Gebruikte variabelen:
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS

        [Test]
        [Category("SSSSSS")]
        public void Demo01()

        //Activeren met ID-Check (evt met sms-code na timeout ID-check)
        //activatie zonder fysiek aanbieden rijbewijs
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();


            DigiDAppActiverenPagina_AP088
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerActiverenMetSMS();


            DigiDAppActiverenPagina_AP044
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .EnterSMSCode("SSSSSS");

            ToestemmingBerichtenPagina_AP021
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopJa();

            DigiDAppActiverenPaginaSMS_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
                .Load();

        }

        [Test]
        public void Demo01_RDAFailed()
        //Activeren met SMS na falen ID-check
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            ScannenIdentiteitsbewijsPagina_AP038
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .StartScannen();

            DigiDAppActiverenPagina_AP104
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            DigiDAppActiverenPagina_AP044
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .EnterSMSCode("SSSSSS");

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaSMS_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
                .Load();

        }

        [Test]
        [Category("SSSSSS")]
        public void Demo02()

        //Activeren met ID-Check; bevestigen van eerder geregistreerd e-mail adres
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            ScannenIdentiteitsbewijsPagina_AP038
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .StartScannen();

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ActiveerKnopNee();

            ControleerEmailadres_AP112
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .MailAdresBevestigen();

            EmailAdresPagina_AP108
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VulControleCodeEmail();

            DigiDAppActiverenPaginaRDA_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
                .Load()
                .ControleerOfJuisteTekstWordtGetoond();



        }

        [Test]
        public void Demo03_basis()

        //Activeren account na aanvraag (basis) via website
        //Geschikt voor AppCenter, rijbewijs hoeft niet te worden aangeboden
        {

            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            DigiDAppActiverenPagina_AP045
                .Load()
                .VoerBriefActiveringscodeIn();

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            ToestemmingBerichtenPagina_AP021
               .Load()
               .ActiveerKnopNee();

            DigiDAppActiverenPaginaAccountAndApp_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .ActiveerKnopNeeDankJe();

            IDCheckUitstellenPagina_AP062
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ActiveerKnopNee();

            WelkomstPaginaNaActivatie_AP001
               .Load();


        }

        [Test]
        public void Demo03_sms()
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            DigiDAppActiverenPagina_AP044
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .EnterSMSCode("SSSSSS");

            DigiDAppActiverenPagina_AP045
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VoerBriefActiveringscodeIn();

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .GeldigePincode();

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaAccountAndApp_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNeeDankJe();

            IDCheckUitstellenPagina_AP062
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            WelkomstPaginaNaActivatie_AP001
                .Load();

                

                
                





        }
        



        [Test]
        public void Demo04_IDCheck()
        //Activeren met ID-check (evt. met alleen gn/ww na timeout ID-check)
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund

        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            ScannenIdentiteitsbewijsPagina_AP038
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .StartScannen();


            ToestemmingBerichtenPagina_AP021
                .Load()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaRDA_AP007
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
               .Load();

        }

        [Test]
        public void Demo04_IDCheckFailed_ActiverenGNWW()
        ///Activeren met alleen gn/ww na falen ID-check
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund

        {

            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            DigiDAppActiverenPagina_AP089
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ScanOverslaan();

            ToestemmingBerichtenPagina_AP021
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopJa();

            DigiDAppActiverenPaginaPassword_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
               .Load();



        }

        [Test]
        public void Demo05()
        //Aanvragen en actievern account
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund
        {

            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .ActiveerKnopGeenDigiD();

            GeenDigiDPagina_AP046
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopDigiDAanvragen();

            //app.Repl();

            DigiDAanvragenPagina_AP079
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .InvullenBSN("SSSSSSSSS")
                .InvullenGebDatum("PPPPPPPP")
                .InvullenPostcode("SSSSSS")
                .InvullenHuisnummer("1")
                .ActiveerKnopVolgende();

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            EmailAdresPagina_AP080
                .Load()
                .ActiveerKnopOverslaan()
                .WachtOpPopUp()
                .BevestigGeenMail();

            DigiDAppActiverenPagina_AP069
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopOverslaan();

            WachtOpBriefStatusPagina_AP001
                .Load()
                .ActiveerKnopActiveren();

            DigiDAppActiverenPagina_AP012
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopActiveren();

            PincodeInvoerenPagina_AP016
                .Load()
                .GeldigePincode();

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ActiveerKnopJa();

            DigiDAppActiverenPagina_AP045
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VoerBriefActiveringscodeIn();

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaLetter_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
              .Load();
        }

        [Test]
        public void Demo06()
        //Activeren met ID-check na handmatige invoer documentgegevens
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund
        {

            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            GeldigNedPaspoortOfIdentiteitskaartPagina_AP098
                .Load()
                .ActiveerKnopJa();

            app.Repl();

            GegevensInvullenPagina_AP109
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .GegevensInvullenIdentiteitskaart()
                .InvullenDocumentnummer("SSSSSSSSS")
                .InvullenDocumentGeldigTot("PPPPPPPP")
                .InvullenGebDatum("PPPPPPPP")
                .ActiveerKnopVolgende();

            ScannenIdentiteitsbewijsPagina_AP107
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();


            ToestemmingBerichtenPagina_AP021
                .Load()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaRDA_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
               .Load();

        }

        [Test]
        public void Demo07()
        //NIET VOOR APPCENTER
        //rijbewijs moet onder device worden gelegd voordat test wordt gerund
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopJa();

            EmailAdresToevoegenPagina_AP101
               .Load()
               .ControleerOfJuisteTekstWordtGetoond()
               .ActiveerKnopJa();

            EmailAdresPagina_AP080
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .VulMailAdres()
                .ActiveerKnopVolgende();

            EmailAdresPagina_AP081
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                //.OpenenHelpPagina()
                //.ControleerTekstHelpPagina()
                //.SluitHelpPagina()
                .VulControleCodeEmail();


            DigiDAppActiverenPaginaRDA_AP007
                .Load()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
               .Load();




        }

        [Test]
        [Category("FlowsVanuitMenu")]
        public void CheckMenuItems()
        {
            WelkomstPagina_AP001
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .ControleerStatusApp()
                 .StartActivatie();

            DigiDAppOpAnderApparaatPagina
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopNee();

            DigiDAppActiverenPagina_AP043
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .Inloggen("SSSSS", "SSSSS");

            KiesUwPincodePagina_AP006
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenenHelpPagina()
                .ControleerTekstHelpPagina()
                .SluitHelpPagina()
                .GeldigePincode();

            DigiDAppActiverenPagina_AP086
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopVolgende();


            DigiDAppActiverenPagina_AP088
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerActiverenMetSMS();


            DigiDAppActiverenPagina_AP044
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .EnterSMSCode("SSSSSS");

            ToestemmingBerichtenPagina_AP021
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopJa();

            DigiDAppActiverenPaginaSMS_AP007
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .ActiveerKnopBegrepen();

            WelkomstPaginaNaActivatie_AP001
                .Load()
                .OpenMenu();

            MenuPagina_AP036
                .Load()
                .StartIDCheck();


            IDCheckVanuitMenuPagina_AP090
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PaginaSluiten();

            WelkomstPaginaNaActivatie_AP001
                .Load()
                .OpenMenu();

           MenuPagina_AP036
                .Load()
                .OpenBerichtenPagina();

            PincodeInvoerenPagina_AP016
                .Load()
                .GeldigePincode();

            BerichtenCentrumPagina_AP022
                .Load()
                .PaginaSluiten();

            WelkomstPaginaNaActivatie_AP001
               .Load()
               .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenMijnDigiD();

            MijnDigiDPagina_AP106
                .Load()
                .EmailAdresToevoegen();

            //PincodeInvoerenPagina_AP016
            //    .Load()
            //    .GeldigePincode();

            UwEmailAdresPagina_AP114
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .EmailAdresAkkoord();

            

            

            MijnDigiDPagina_AP106
                .Load()
                .GebruiksgeschiedenisOpenen();

            //PincodeInvoerenPagina_AP016
            //    .Load()
            //    .GeldigePincode();

            GebruiksgeschiedenisPagina_AP115
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PaginaSluiten();

            WelkomstPaginaNaActivatie_AP001
                .Load()
                .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenMijnDigiD();




            MijnDigiDPagina_AP106
                .Load()
                .OpenPincodeWijzigen();

            PincodeWijzigenPagina_AP095
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PincodeAppWijzigen();

            PincodeWijzigenPagina_AP073
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PincodeVergeten();

            PincodeVergetenPagina_AP099
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VorigePagina();

            PincodeWijzigenPagina_AP073
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PincodeWijzigen();

            PincodeInvoerenPagina_AP016
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .GeldigePincode()
                .PincodeWijzigen()
                .PincodeWijzigen();

            PincodeGewijzigdPagina_AP075
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .PincodeGewijzigdAkkoord();




            WelkomstPaginaNaActivatie_AP001
               .Load()
               .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenInstellingenPagina();

            InstellingenPagina_AP068
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .OpenTaal();

            TaalPagina_AP096
                .Load()
                .TaalInEngels()
                .ControleerWijzigingTaalEngels()
                .TaalInNederlands()
                .ControleerWijzigingTaalNederlands()
                .PaginaSluiten();

            WelkomstPaginaNaActivatie_AP001
               .Load()
               .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenInstellingenPagina();

            InstellingenPagina_AP068
                 .Load()
                 .ControleerOfJuisteTekstWordtGetoond()
                 .OpenDonkereModus();

            DonkereModusPagina_AP097
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .DonkereModusAan()
                .DonkereModusUit()
                .DonkereModusAutomatisch()
                .PaginaSluiten();

           WelkomstPaginaNaActivatie_AP001
               .Load()
               .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenHulpEnInfoPagina();

            HulpEnInformatiePagina_AP092
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenVeelgesteldeVragen();

            VeelGesteldeVragenPagina_AP065
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VorigePagina();

            HulpEnInformatiePagina_AP092
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OpenContact();

            ContactPagina_AP093
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VorigePagina();

            HulpEnInformatiePagina_AP092
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .OverDeDigiDApp();

            OverDeDigiDAppPagina_AP064
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .VorigePagina();

            HulpEnInformatiePagina_AP092
                .Load()
                .OpenSourceBibliotheken();

            OpenSourceBibliothekenPagina_AP085
                .Load()
                .VorigePagina();

            HulpEnInformatiePagina_AP092
                .Load()
                .PaginaSluiten();

            WelkomstPaginaNaActivatie_AP001
               .Load()
               .OpenMenu();

            MenuPagina_AP036
                .Load()
                .OpenInstellingenPagina();

            InstellingenPagina_AP068
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .Deactiveren();

            DeactiverenPagina_AP010
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .DeActiverenBevestigen()
                .ControleerOfJuisteTekstWordtGetoondInPopUp()
                .DeActiveren();

            DigiDAppDeactiverenPagina_AP077
                .Load()
                .ControleerOfJuisteTekstWordtGetoond()
                .DeActiverenBevestigen();

            WelkomstPagina_AP001
               .Load();

               }

    }

}
