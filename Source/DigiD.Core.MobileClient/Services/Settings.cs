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
using DigiD.Common.BaseClasses;
using DigiD.Common.Enums;
using DigiD.Common.Settings;
using DigiD.Helpers;
using DigiD.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(Settings))]
namespace DigiD.Services
{
    public class Settings : BaseAppSettings, IMobileSettings
    {
        public bool? NotificationPermissionSet
        {
            get => GetValue((bool?)null);
            set => SetValue(value);
        }

        public string NotificationToken
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string NewNotificationToken
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string SymmetricKey
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string AppId
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string MaskCode
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public ActivationMethod ActivationMethod
        {
            get => Enum.Parse<ActivationMethod>(Store.FindValueForKey(nameof(ActivationMethod), ActivationMethod.Unsupported.ToString()).Replace("\"", string.Empty));
            set => Store.Insert(value.ToString(), nameof(ActivationMethod));
        }

        public ActivationStatus ActivationStatus
        {
            get => Enum.Parse<ActivationStatus>(Store.FindValueForKey(nameof(ActivationStatus), ActivationStatus.NotActivated.ToString()).Replace("\"", string.Empty));
            set => Store.Insert(value.ToString(), nameof(ActivationStatus));
        }

        public LoginLevel LoginLevel
        {
            get => Enum.Parse<LoginLevel>( Store.FindValueForKey(nameof(LoginLevel), LoginLevel.Unknown.ToString()).Replace("\"", string.Empty));
            set => Store.Insert(value.ToString(), nameof(LoginLevel));
        }

        public override void Reset()
        {
            base.Reset();

            SigningHelper.GetService().RemoveKeyPair();
            App.RegisterServices(AppMode.Normal);
        }
    }
}

