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
﻿using System.Collections.Generic;
using System.Linq;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Browser = Xamarin.Essentials.Browser;
using MenuItem = DigiD.Models.MenuItem;

namespace DigiD.ViewModels
{
    public class AP106ViewModel : BaseViewModel
    {
        public string MyDigiDTitle => AppResources.AP106_MijnDigiDButtonText;

        private string _accessibilityText;
        public string AccessibilityText
        {
            get
            {
                if (string.IsNullOrEmpty(_accessibilityText))
                {
                    _accessibilityText = MyDigiDTitle;
                }
                return _accessibilityText + $", {AppResources.AccessibilityMenuItemLinkText}";
            }
            set => _accessibilityText = value;
        }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public AP106ViewModel()
        {
            PageId = "AP106";
            HeaderText = AppResources.AP106_Header;
            HasBackButton = true;

            MenuItems.Add(new MenuItem
            {
                Title = AppResources.AppMenuManageEmail,
                Icon = "resource://DigiD.Resources.digid_icon_menu_email.svg",
                Action = async () => await SessionHelper.StartSession(() => EmailHelper.Manage(false))
            });

            MenuItems.Add(new MenuItem
            {
                Title = AppResources.AppMenuUsageHistory,
                Icon = "resource://DigiD.Resources.digid_icon_menu_app_gebruiksgeschiedenis.svg",
                Action = async () => await SessionHelper.StartSession(async () =>
                {
                    var items = await DependencyService.Get<IUsageHistoryService>().GetUsageHistory();
                    await NavigationService.PushAsync(new UsageHistoryViewModel(items));
                })
            });

            var menuItem = new MenuItem
            {
                Title = AppResources.AppMenuChangePincode,
                Icon = "resource://DigiD.Resources.digid_icon_pincode_veranderen.svg",
            };

            if (App.HasNfc)
                menuItem.TargetViewModel = typeof(ConfirmChangePinViewModel);
            else
                menuItem.Action = async () => { await ChangePinHelper.StartChangePIN(); };

            MenuItems.Add(menuItem);

            if (DependencyService.Get<IMobileSettings>().ActivationMethod != ActivationMethod.RequestNewDigidAccount)
            {
                MenuItems.Add(new MenuItem
                {
                    Title = AppResources.AppMenuTwoFactor,
                    Icon = "resource://DigiD.Resources.digid_icon_menu_2fa.svg",
                    Action = async () => await SessionHelper.StartSession(async () =>
                    {
                        var twoFactorEnabled = await DependencyService.Get<IAccountInformationServices>().GetTwoFactorStatus();

                        if (twoFactorEnabled.IsAppOnlyAccount)
                            await NavigationService.PushAsync(new AP119ViewModel());
                        else
                            await NavigationService.PushAsync(new AP120ViewModel(twoFactorEnabled.Enabled));
                    })
                });
            }
        }

        public AsyncCommand OpenMyDigiDCommand => new AsyncCommand(async () =>
        {
            if (string.IsNullOrEmpty(AppSession.MyDigiDUrl))
            {
                App.Apps = await DependencyService.Get<IGeneralServices>().GetServices();

                var app = App.Apps.Services.FirstOrDefault(x => x.Name == Common.Constants.DemoConstants.MijnDigidAppName);
                if (app != null)
                    AppSession.MyDigiDUrl = app.Url;
            }

            if (!string.IsNullOrEmpty(AppSession.MyDigiDUrl))
            {
                if (await Launcher.CanOpenAsync(AppSession.MyDigiDUrl))
                    await Launcher.OpenAsync(AppSession.MyDigiDUrl);
            }
        });
    }
}
