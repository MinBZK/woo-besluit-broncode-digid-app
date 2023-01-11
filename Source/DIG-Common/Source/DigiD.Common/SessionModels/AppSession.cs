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
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Common.SessionModels
{
    public static class AppSession
    {
        public static Process Process { get; set; }
        public static AppMode AppMode { get; set; }
        public static string AuthenticationSessionId { get; set; }
        public static Func<Task> ManagementAction { get; set; }
        public static AccountStatusResponse AccountStatus { get; private set; } = new AccountStatusResponse();
        public static string MyDigiDUrl { get; set; }
        public static bool IsAppActivated { get; set; } 

        public static void SetAccountStatus(AccountStatusResponse response)
        {
            AccountStatus = response;

            if (response == null)
                return;

            if (!response.TwoFactorEnabled)
                AccountStatus.OpenTasks.Add(AccountTask.TwoFactor);
            if (response.EmailStatus == EmailStatus.None)
                AccountStatus.OpenTasks.Add(AccountTask.Email);
            if (DependencyService.Get<IMobileSettings>().LoginLevel < LoginLevel.Substantieel)
                AccountStatus.OpenTasks.Add(AccountTask.RDA);
            if (!DependencyService.Get<IPushNotificationService>().NotificationsEnabled())
                AccountStatus.OpenTasks.Add(AccountTask.Notification);
        }

        public static event EventHandler MenuItemChanged;

        public static void MenuItemChange()
        {
            MenuItemChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void Reset()
        {
            Process = Process.NotSet;
        }
    }
}
