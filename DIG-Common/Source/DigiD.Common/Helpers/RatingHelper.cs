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
using System.Threading.Tasks;
using Plugin.StoreReview;
using Xamarin.Essentials;
using Device = Xamarin.Forms.Device;

namespace DigiD.Common.Helpers
{
    public static class RatingHelper
    {
        public static async Task RequestRating(bool forced = false)
        {
            var testMode = true;

#if PROD
            testMode = false;
#endif

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    if (forced)
                        CrossStoreReview.Current.OpenStoreReviewPage("SSSSSSSSSSSSSSSSSSSSSSSSSS");
                    else
                    {
                        try
                        {
                            await CrossStoreReview.Current.RequestReview(testMode);
                        }
                        catch (Exception)
                        {
                            //Do nothing if exception is raised
                        }
                    }
                    break;
                case Device.macOS:
                    await Launcher.OpenAsync(new System.Uri("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"));
                    break;
                case Device.iOS:
                    if (forced)
                        CrossStoreReview.Current.OpenStoreReviewPage("SSSSSSSSSS");
                    else
                        await CrossStoreReview.Current.RequestReview(testMode);
                    break;
                default:
                    await CrossStoreReview.Current.RequestReview(testMode);
                    break;
            }
        }
    }
}
