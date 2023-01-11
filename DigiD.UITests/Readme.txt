# Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
# gericht is op transparantie en niet op hergebruik. Hergebruik van 
# de broncode is toegestaan onder de EUPL licentie, met uitzondering 
# van broncode waarvoor een andere licentie is aangegeven.
#
# Het archief waar dit bestand deel van uitmaakt is te vinden op:
#   https://github.com/MinBZK/woo-besluit-broncode-digid-app
#
# Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
#   https://www.ncsc.nl/contact/kwetsbaarheid-melden
# onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
#
# Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
#
# This code has been disclosed in response to a request under the Dutch
# Open Government Act ("Wet open Overheid"). This implies that publication 
# is primarily driven by the need for transparence, not re-use.
# Re-use is permitted under the EUPL-license, with the exception 
# of source files that contain a different license.
#
# The archive that this file originates from can be found at:
#   https://github.com/MinBZK/woo-besluit-broncode-digid-app
#
# Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
#   https://www.ncsc.nl/contact/kwetsbaarheid-melden
# using the reference "Logius, publicly disclosed source code DigiD-App" 
#
# Other questions regarding this Open Goverment Act decision may be
# directed via email to open@logius.nl
#
﻿------
README
------

See also: https://SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS


├── Pageobjects                  # per page     
│   ├── WelkomPage.cs
│   ├── ToeslagenPage.cs
│   └── ...
├── Tests / Regressiontests.     # testclasses per page/usecase
│   ├── WelkomPageTests.cs                                                     
│   ├── ToeslagenPageTests.cs
│   └── ...
├── APP2Testbase.cs              # base class

1. Add the necessary Automation id's to the pages (xml) in the main (.Net) project.
   Hint: Look under views/pages.

2. Modify or replace current AppInitializer.cs

- Enable Localscreenshots.
- Set global wait times.
- Use AppDataMode.Clear.

    if (platform == Platform.Android)
    {
        return ConfigureApp
            .Android
            .WaitTimes(new WaitTimes())
            .EnableLocalScreenshots()
            .StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
    }

    return ConfigureApp
        .iOS
        .WaitTimes(new WaitTimes())
        .EnableLocalScreenshots()
        .StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);

3. Add any necesesary compiler options, for example: IOS and ANDROID to be able to run aklkl tests on both.

4. Create a pageobject for each page in the app which needs to be accessed by a UITest.
   See example: Samples/SamplePage.cs. (Install TestApp.Samples)

5. Create one UITest file per page in the app or alternatively one UITest file per UseCase.
   See example: Samples/SamplePageTest.cs (Install TestApp.Samples)

6. Install the following packages to install the backdoors:
    .Net:  Install TestAAP.Helpers.Net
    .Droid: Install TestAPP.Helpers.Droid
    .iOS: Install TestApp.Helpers.iOS

   And add Compile option ENABLE_TEST_CLOUD to each project to enable them.

    ** Note: Both Droid and iOS have a dependancy on TestAAP.Helpers.Net

7. Modify ioS:
   Open AppDelegate.cs and make it partial
        Hint: "public partial class AppDelegate : FormsApplicationDelegate"

   Ensure that the Xamarin Test Cloud agent will be started.

        #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start(); // Code for starting up the Xamarin Test Cloud Agent
        #endif

8. Modify Droid:
   Open MainActivity.cs and make it a partial class
        Hint: "public partial class MainActivity : FormsAppCompatActivity"

8. Optionally enable the DataLab backdoors
      Add Compile option ENABLE_DATALAB_BACKDOORS to the main (.Net) project.

9. Create and run tests locally, starting with iOS instead of Droid is often smarter because of small differences in Marked.

10. Run tests in App Center (TestCloud)
   https://docs.microsoft.com/en-us/appcenter/test-cloud/frameworks/uitest/troubleshooting/

   You can run your tests locally with a script from terminal. Generate script fromn AppCenter and modify as necessary to make sure the script points to the right directories.
   Specify full pathnames to run from anywhere or relative pathnames when planning for reuse.

   example, How to run MKT UITests against iOS Debug in TestCloud:
            
    - Build Debug|iPhone Generic Device 
    - Run the following command from Terminal

    ```
    cd Repos/SSSSSSSSSSSSSSSSSSSSSSS
    appcenter test run uitest --app "belastingdienst/SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"
        --devices SSSSSSSS
        --app-path ../iOS/bin/Debug/MKT.IOS.ipa
        --test-series "launch-tests"
        --locale "en_US"
        --build-dir ../UITests/bin/Debug
        --uitest-tools-dir ../UITests/bin/Debug
    ```
