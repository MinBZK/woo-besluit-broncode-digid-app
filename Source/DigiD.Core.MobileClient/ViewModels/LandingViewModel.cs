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
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using DigiD.Services;
using DigiD.UI.Popups;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class LandingViewModel : BaseViewModel
    {
        public bool IsLetterActivation { get; set; }
        public string CancelText { get; set; }
        public ICommand CancelCommand { get; set; }
        public bool ExplanationLinkVisible { get; private set; }
        public bool ShowPreferenceOptions { get; set; }

        // tbv uitleg bij geactiveerde app
        public bool IsAppActivated => DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated;

        public string ImageSource
        {
            get
            {
                if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Pending)
                    return "resource://DigiD.Resources.afbeelding_wachten_op_brief.svg";
                if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated)
                    return "resource://DigiD.Resources.digid_afbeelding_landing_app_is_geactiveerd.svg";

                return "resource://DigiD.Resources.digid_afbeelding_landing_activeer_de_app.svg";
            }
        }
        private bool _init;

#if !PROD
        public ICommand ChangeLetterRequestDate => new Command(() =>
        {
            var difference = Math.Ceiling((DependencyService.Get<IPreferences>().LetterRequestDate
                .AddDays(App.Configuration.LetterRequestDelay).ToLocalTime() - DateTime.UtcNow).TotalDays);
            int daysToSubtract;

            if (difference >= 0)
                daysToSubtract = App.Configuration.LetterRequestDelay + 1;
            else
                daysToSubtract = -App.Configuration.LetterRequestDelay;

            DependencyService.Get<IPreferences>().LetterRequestDate = DateTime.Now.AddDays(-daysToSubtract);

        });
#endif
        // bovenstaande is om te kunnen demo' en dat door 4x op de 'Wacht op brief'-image te tappen de datum wanneer de brief is aangevraagd
        // te verzetten zodat we de logica mbt enabled buttons etc kunnen laten zien.

        public AsyncCommand OpenAP087Command => new AsyncCommand(async () =>
        {
            await NavigationService.PushAsync(new AP087ViewModel());
        });

        public LandingViewModel()
        {
            SetTextBaseOnActivationStatus();
            IsLetterActivation = DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Pending;
            PageId = "AP001";
            _init = true;

            DemoBarPressed = new Command(() =>
            {
                NavigationService.OpenPopup(new CardStatePopup());
            });
        }

        public ICommand LogoutCommand { get; private set; }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await PiwikHelper.Init();

            ShowPreferenceOptions = !App.IsNewActivated && AppSession.AccountStatus?.OpenTasks.Count > 0 && DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated && DependencyService.Get<IPreferences>().ShowPreferenceOptions;

            if (!_init)
                SetTextBaseOnActivationStatus();
            else
                await SetAdditionalPage();

            DeviceLockService.SetTimer();

            _init = false;
        }

        private static async Task SetAdditionalPage()
        {
            var checkedVersion = DependencyService.Get<IPreferences>().CheckedVersion;
            var currentVersion = AppInfo.Version;
            Func<Task> stopAction = null;

            if (checkedVersion.Major < currentVersion.Major || checkedVersion.Minor < currentVersion.Minor)
            {
                stopAction = async () =>
                {
                    DependencyService.Get<IPreferences>().CheckedVersion = currentVersion;

                    if (App.HasNfc &&
                        DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated &&
                        DependencyService.Get<IMobileSettings>().LoginLevel == LoginLevel.Midden)
                    {
                        await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.IDcheckStart), false));
                    }
                };
            }

            if (WhatsNewHelper.MustShow)
                await WhatsNewHelper.Show(stopAction);
            else if (stopAction != null)
                await stopAction.Invoke();
        }

        private void SetTextBaseOnActivationStatus()
        {
            ExplanationLinkVisible = false;

            switch (DependencyService.Get<IMobileSettings>().ActivationStatus)
            {
                case ActivationStatus.Activated:
                    ButtonText = AppResources.LandingActivatedButton;
                    HeaderText = App.IsNewActivated ? AppResources.LandingActivatedFooter : string.Empty;
                    ExplanationLinkVisible = true;
                    LogoutCommand = new Command(DeviceLockService.LockApp);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        CanExecute = false;
                        await NavigationService.PushAsync(new VerificationCodeViewModel());
                        CanExecute = true;
                    }, () => !(HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession) && CanExecute);
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        CanExecute = false;
                        await NavigationService.PushModalAsync(new MoreLoginInformationViewModel(), nav: false);

                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case ActivationStatus.NotActivated:
                    FooterText = AppResources.LandingNotActivatedFooter;
                    ButtonText = AppResources.LandingNotActivatedButton;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        CanExecute = false;
#if !DEBUG
                        if (DependencyService.Get<Common.Services.ISecurityService>().IsDebuggerAttached)
                        {
                            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("debugger_attached", new System.Collections.Generic.Dictionary<string, string>
                            {
                                {"flow","activation"},
                                {"user_app_id",DependencyService.Get<IMobileSettings>().AppId},
                                {"instance_id",DependencyService.Get<IGeneralPreferences>().InstanceId}
                            });

                            await NavigationService.ShowMessagePage(MessagePageType.ActivationFailed);
                            return;
                        }
#endif
                        await NavigationService.PushAsync(new ActivationIntro1ViewModel());
                        CanExecute = true;
                    }, () => CanExecute);

                    break;
                case ActivationStatus.Pending:
                    FooterText = AppResources.LandingPendingFooter;
                    ButtonText = AppResources.LandingPendingButton;
                    LogoutCommand = new Command(DeviceLockService.LockApp);
                    HeaderText = AppResources.LandingPendingHeader;
                    CancelText = AppResources.NoLetterReceived;
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        CanExecute = false;
                        await ContinueLetterActivation(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        CanExecute = false;
                        await ContinueLetterActivation(false);
                        CanExecute = true;
                    }, () => CanExecute);

                    break;
            }
        }

        private async Task ContinueLetterActivation(bool requestNewLetter)
        {
            await SessionHelper.StartSession(async () =>
            {
                DialogService.ShowProgressDialog();

                if (!requestNewLetter)
                {
                    var response = await DependencyService.Get<IEnrollmentServices>().InitSessionActivationCode(new ActivationCodeSessionRequest
                    {
                        AppId = DependencyService.Get<IMobileSettings>().AppId,
                        RequestNewLetter = false
                    });

                    switch (response.ApiResult)
                    {
                        case ApiResult.Ok:
                            HttpSession.AppSessionId = response.SessionId;
                            await NavigationService.PushAsync(new ActivationLetterViewModel(false));
                            break;
                        default:
                            await NavigationService.ShowMessagePage(MessagePageType.ActivationFailed);
                            break;
                    }
                }
                else
                    await DependencyService.Get<INavigationService>().PushAsync(new NoLetterReceivedViewModel());
                
                DialogService.HideProgressDialog();
            });
        }

        public override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
