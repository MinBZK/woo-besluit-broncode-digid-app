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
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class AP087ViewModel : BaseNotificationsSettingsViewModel
    {
        public List<PreferenceBlock> Blocks { get; set; }
        public bool HideOptions { get; set; }

        public string HideOptionsText => Blocks.Count == 1
            ? MobileAppResources.AP087_HideOption_Text
            : MobileAppResources.AP087_HideOptions_Text;

        public new string FooterText => Blocks.Count == 1
            ? MobileAppResources.AP087_Message_Single
            : MobileAppResources.AP087_Message_Multiple;

        public Command HideOptionsCommand { get; }

        public AP087ViewModel() : base(false)
        {
            HeaderText = MobileAppResources.AP087_Header;

            HasBackButton = true;
            HideOptionsCommand = new Command(() =>
            {
                HideOptions = !HideOptions;
                DependencyService.Get<IPreferences>().ShowPreferenceOptions = !HideOptions;
            });
        }

        public override async void OnAppearing()
        {
            DialogService.ShowProgressDialog();
            var status = await DependencyService.Get<IAccountInformationServices>().GetAccountStatus();
            AppSession.SetAccountStatus(status);

            var blocks = new List<PreferenceBlock>();

            if (AppSession.AccountStatus.OpenTasks.Contains(AccountTask.RDA))
            {
                blocks.Add(new PreferenceBlock
                {
                    Header = MobileAppResources.AP087_Item_IDCheck_Header,
                    Message = MobileAppResources.AP087_Item_IDCheck_Message,
                    ButtonText = MobileAppResources.AP087_Item_IDCheck_ButtonText,
                    Command = new AsyncCommand(async () =>
                    {
                        if (!App.HasNfc)
                        {
                            await NavigationService.PushAsync(new IdCheckNoNfcViewModel());
                        }
                        else
                        {
                            await SessionHelper.StartSession(async () =>
                            {
                                var model = new ConfirmModel(ConfirmActions.IDcheckStartFromMenu);
                                await NavigationService.PushAsync(new ConfirmViewModel(model, false));
                            });
                        }
                    })
                });
            }

            if (AppSession.AccountStatus.OpenTasks.Contains(AccountTask.Email))
            {
                blocks.Add(new PreferenceBlock
                {
                    Header = MobileAppResources.AP087_Item_Email_Header,
                    Message = MobileAppResources.AP087_Item_Email_Message,
                    ButtonText = MobileAppResources.AP087_Item_Email_ButtonText,
                    Command = new AsyncCommand(async () =>
                    {
                        await SessionHelper.StartSession(() => EmailHelper.Manage(true));
                    })
                });
            }

            if (AppSession.AccountStatus.OpenTasks.Contains(AccountTask.Notification))
            {
                blocks.Add(new PreferenceBlock
                {
                    Header = MobileAppResources.AP087_Item_Messages_Header,
                    Message = MobileAppResources.AP087_Item_Messages_Message,
                    ButtonText = MobileAppResources.AP087_Item_Messages_ButtonText,
                    Command = OpenSettingsCommand
                });
            }

            if (AppSession.AccountStatus.OpenTasks.Contains(AccountTask.TwoFactor))
            {
                blocks.Add(new PreferenceBlock
                {
                    Header = MobileAppResources.AP087_Item_2fa_Header,
                    Message = MobileAppResources.AP087_Item_2fa_Message,
                    ButtonText = MobileAppResources.AP087_Item_2fa_ButtonText,
                    Command = new AsyncCommand(async () =>
                    {
                        await NavigationService.PushAsync(new AP120ViewModel(false));
                    })
                });
            }

            Blocks = blocks;

            if (Blocks.Count == 0)
                await NavigationService.GoBack();

            base.OnAppearing();

            DialogService.HideProgressDialog();
        }
    }

    public class PreferenceBlock : BindableObject
    {
        public string Header { get; set; }
        public string Message { get; set; }
        public string ButtonText { get; set; }
        public ICommand Command { get; set; }
    }
}
