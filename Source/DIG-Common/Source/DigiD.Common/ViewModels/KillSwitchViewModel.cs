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
using System.Globalization;
using DigiD.Common.BaseClasses;
using DigiD.Common.Http.Enums;
using DigiD.Common.Models.ResponseModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace DigiD.Common.ViewModels
{
    public class KillSwitchViewModel : CommonBaseViewModel
    {
        private readonly VersionCheckResponse _response;

        public override string PageIntroHelp => $"{HeaderText} {FooterText} {ButtonText}";
        public override string PageIntroName => $"{HeaderText}";
        public string ImageSource { get; set; }
        public bool CancelButtonVisible { get; set; }

#if A11YTEST
        public KillSwitchViewModel() : this(new VersionCheckResponse { Action = "update_warning" }) { }
#endif

        public KillSwitchViewModel(VersionCheckResponse response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));

            switch (response.VersionAction)
            {
                case ApiResult.ForceUpdate:
                    PageId = "AP221";
                    HeaderText = AppResources.AppForceUpdateHeader;
                    ButtonText = AppResources.AppForceUpdateAction;
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.KillswitchForceUpdate, AppInfo.Name);
                    ImageSource = "resource://DigiD.Common.Resources.digid_afbeelding_update_verplicht.svg?assembly=DigiD.Common";

                    if (!string.IsNullOrEmpty(response.UpdateURL))
                        ButtonCommand = new AsyncCommand(async () => await Launcher.OpenAsync(new Uri(response.UpdateURL)));
                    break;

                case ApiResult.Kill:
                    PageId = "AP222";
                    HeaderText = AppResources.AppKillHeader;
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.KillswitchKill, AppInfo.Name);
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_failed.svg?assembly=DigiD.Common";
                    break;
                case ApiResult.UpdateWarning:
                    NavCloseCommand = CancelCommand;
                    CancelButtonVisible = true;
                    PageId = "AP223";
                    HeaderText = AppResources.AppUpdateWarningHeader;
                    ButtonText = AppResources.AppUpdateWarningAction;
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.KillswitchUpdateWarning, AppInfo.Name);
                    ImageSource = "resource://DigiD.Common.Resources.digid_afbeelding_update_niet_verplicht.svg?assembly=DigiD.Common";
                    
                    if (!string.IsNullOrEmpty(response.UpdateURL))
                        ButtonCommand = new AsyncCommand(async () => await Launcher.OpenAsync(new Uri(response.UpdateURL)));
                    break;
            }
        }

        public override bool OnBackButtonPressed()
        {
            return _response.VersionAction != ApiResult.UpdateWarning;
        }

        public IAsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
               {
                   CanExecute = false;
                   await NavigationService.PopCurrentModalPage();
                   CanExecute = true;
               }, () => _response.VersionAction == ApiResult.UpdateWarning);
            }
        }
    }
}

