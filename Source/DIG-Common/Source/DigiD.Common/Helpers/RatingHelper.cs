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
