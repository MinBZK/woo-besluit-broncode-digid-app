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
﻿using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace DigiD.ViewModels
{
    public class ContactViewModel : BaseViewModel
    {
        public const string ContactViaPhone = "phone";
        public const string ContactViaTwitter = "twitter";
        public const string ContactViaContactForm = "contactForm";
        public const string ContactViaDigiDWebsite = "digidwebsite";

        private const string Key = "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS";

        private readonly ObfuscatedString _contactViaPhoneUri = new ObfuscatedString(Key, "SSSSSSSSSSSSSSSSSSSSSSSS"); //SSSSSSSSSSSSSSSS
        private readonly ObfuscatedString _contactViaTwitterUri = new ObfuscatedString(Key, "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"); //twitter://user?screen_name=digidwebcare
        private readonly ObfuscatedString _contactViaTwitterWebUri = new ObfuscatedString(Key, "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"); //https://twitter.com/digidwebcare
        private readonly ObfuscatedString _contactViaDigiDWebsiteUri = new ObfuscatedString(Key, "SSSSSSSSSSSSSSSSSSSSSSSSSSSS"); //https://www.digid.nl
        
        public ContactViewModel()
        {
            PageId = "AP093";
            HeaderText = AppResources.ContactHeader;
            HasBackButton = true;
            NavCloseCommand = new AsyncCommand(async () => await NavigationService.PopToRoot());
        }
        public ICommand ZoekContactVia => new AsyncCommand<string>(async contactVia =>
        {
            if (!CanExecute)
                return;

            CanExecute = false;

            switch (contactVia)
            {
                case ContactViaPhone:
                    PiwikHelper.Track("contact", "phone", "AP093");
                    await Launcher.OpenAsync(_contactViaPhoneUri.ToString());
                    break;
                case ContactViaTwitter:
                    PiwikHelper.Track("contact", "twitter", "AP093");
                    await OpenAppOrWebsite(_contactViaTwitterUri.ToString(), _contactViaTwitterWebUri.ToString());
                    break;
                case ContactViaContactForm:
                    PiwikHelper.Track("contact", "contactform", "AP093");
                    await NavigationService.PushModalAsync(new WebViewViewModel(AppResources.ContactFormUrl));
                    break;
                case ContactViaDigiDWebsite:
                    PiwikHelper.Track("contact", "website", "AP093");
                    await Launcher.OpenAsync(_contactViaDigiDWebsiteUri.ToString());
                    break;
            }

            CanExecute = true;
        }, u => CanExecute);


        private static async Task OpenAppOrWebsite(string contactViaAppUri, string contactViaWebUri)
        {
            if (!await Launcher.TryOpenAsync(contactViaAppUri))
                await Launcher.OpenAsync(contactViaWebUri);
        }
    }
}
