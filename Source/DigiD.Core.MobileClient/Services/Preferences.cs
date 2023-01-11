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
using System.Diagnostics.CodeAnalysis;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Settings;
using DigiD.Services;

[assembly: Xamarin.Forms.Dependency(typeof(Preferences))]
namespace DigiD.Services
{
    public class Preferences : IPreferences
    {
        [NotNull]
        public Version CheckedVersion
        {
            get
            {
                if (Xamarin.Essentials.Preferences.ContainsKey(nameof(CheckedVersion)))
                {
                    var value = Xamarin.Essentials.Preferences.Get(nameof(CheckedVersion), Xamarin.Essentials.AppInfo.Version.ToString());
                    return new Version(value);
                }

                return Xamarin.Essentials.AppInfo.Version;
            }
            set => Xamarin.Essentials.Preferences.Set(nameof(CheckedVersion), value.ToString());
        }

        public DateTimeOffset LetterRequestDate
        {
            get => Xamarin.Essentials.Preferences.ContainsKey(nameof(LetterRequestDate)) ? Xamarin.Essentials.Preferences.Get(nameof(LetterRequestDate), 0L).FromUnixTime() : DateTimeOffset.MinValue;
            set => Xamarin.Essentials.Preferences.Set(nameof(LetterRequestDate), value.ToUnixTimeSeconds());
        }

        public bool LetterReRequestAllowed
        {
            get => Xamarin.Essentials.Preferences.ContainsKey(nameof(LetterReRequestAllowed)) ? Xamarin.Essentials.Preferences.Get(nameof(LetterReRequestAllowed), true) : true;
            set => Xamarin.Essentials.Preferences.Set(nameof(LetterReRequestAllowed), value);
        }

        public bool NewMessagesAvailable
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(NewMessagesAvailable), false);
            set => Xamarin.Essentials.Preferences.Set(nameof(NewMessagesAvailable), value);
        }

        public bool M17PopupShown
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(M17PopupShown), false);
            set => Xamarin.Essentials.Preferences.Set(nameof(M17PopupShown), value);
        }

        public bool ShowPreferenceOptions 
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(ShowPreferenceOptions), true);
            set => Xamarin.Essentials.Preferences.Set(nameof(ShowPreferenceOptions), value);
        }

        public void Reset()
        {
            LetterRequestDate = DateTimeOffset.MinValue;
            LetterReRequestAllowed = true;
            LetterNotificationDate = DateTimeOffset.MinValue;
            NewMessagesAvailable = false;
            M17PopupShown = false;
        }

        public DateTimeOffset? LetterNotificationDate
        {
            get
            {
                if (Xamarin.Essentials.Preferences.ContainsKey(nameof(LetterNotificationDate)))
                    return Xamarin.Essentials.Preferences.Get(nameof(LetterNotificationDate), 0L).FromUnixTime();

                return null;
            }
            set
            {
                if (value.HasValue)
                    Xamarin.Essentials.Preferences.Set(nameof(LetterNotificationDate), value.Value.ToUnixTime());
                else
                    Xamarin.Essentials.Preferences.Remove(nameof(LetterNotificationDate));
            }
        }
    }
}
