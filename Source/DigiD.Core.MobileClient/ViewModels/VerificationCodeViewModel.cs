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
﻿using DigiD.Common;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.SessionModels;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

#if !DEBUG
using System.Linq;
using System.Security.Cryptography;
using System;
#endif

namespace DigiD.ViewModels
{
    public class VerificationCodeViewModel : BaseViewModel
    {
        public string VerificationCode { get; set; }
        public AsyncCommand LinkCommand => new AsyncCommand(async () =>
        {
            await DependencyService.Get<INavigationService>().PushModalAsync(new MoreLoginInformationViewModel(), nav: false);
        });

#if A11YTEST
        public VerificationCodeViewModel() : this(false) { }
#endif

        public VerificationCodeViewModel()
        {
            PageId = "AP042";
            HeaderText = AppResources.KoppelcodeHeader;
            HasBackButton = true;

            ButtonCommand = new AsyncCommand(async () =>
            {
                HttpSession.VerificationCode = VerificationCode;
                await QRCodeScannerHelper.ShowScannerPage();
            });

            GenerateCode();

            void GenerateCode()
            {
#if !DEBUG
                const string chars = "BCDFGHJLMNPQRSTVWXZ";
                using (var random = new RNGCryptoServiceProvider())
                {
                    var result = new string(Enumerable.Repeat(chars, 4).Select(s => s[RandomInteger(random, s.Length-1)]).ToArray());
                    VerificationCode = result;
                }

                //http://csharphelper.com/blog/2014/08/use-a-cryptographic-random-number-generator-in-c/
                int RandomInteger(RandomNumberGenerator random, int max)
                {
                    const int min = 0;
                    var scale = uint.MaxValue;
                    while (scale == uint.MaxValue)
                    {
                        // Get four random bytes.
                        var four_bytes = new byte[4];
                        random.GetBytes(four_bytes);

                        // Convert that into an uint.
                        scale = BitConverter.ToUInt32(four_bytes, 0);
                    }

                    // Add min to the scaled difference between max and min.
                    return (int)(min + (max - min) *
                        (scale / (double)uint.MaxValue));
                }
#else

                VerificationCode = "ZZZZ";
#endif
            }
        }

        public AsyncCommand HelpCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    await NavigationService.PushModalAsync(new VideoPlayerViewModel(VideoFile.Koppelcode), true, false);
                });
            }
        }
    }
}
