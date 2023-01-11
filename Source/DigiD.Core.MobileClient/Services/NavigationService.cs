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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.BaseClasses;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.RDA.Pages;
using DigiD.Common.RDA.ViewModels;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using DigiD.UI.Pages;
using DigiD.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using BasePopup = DigiD.Common.BaseClasses.BasePopup;

namespace DigiD.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> _pages = new Dictionary<Type, Type>();

#if A11YTEST
        public Dictionary<Type, Type> RegisteredPages => _pages;
#endif

        public NavigationService()
        {
#if ENVIRONMENT_SELECTABLE
            Register<DebugSettingsViewModel, DebugSettingsPage>();
#endif

            Register<PinCodeViewModel, PincodePage>();
            Register<ActivationLoginViewModel, ActivationLoginView>();
            Register<RdaViewModel, RDAPage>();
            Register<NotActivatedPendingViewModel, NotActivatedPendingPage>();
            Register<ActivationIntro1ViewModel, ActivationIntro1Page>();
            Register<ActivationIntro2ViewModel, ActivationIntro2Page>();
            Register<App2AppDisabledViewModel, App2AppDisabledPage>();
            Register<WebserviceConfirmViewModel, WebserviceConfirmPage>();
            Register<WidChangeTransportPinConfirmViewModel, WIDChangeTransportPINConfirmPage>();
            Register<WidScannerViewModel, WIDScannerPage>();
            Register<MessageViewModel, MessagePage>();
            Register<SettingsViewModel, SettingsPage>();
            Register<LandingViewModel, LandingPage>();
            Register<AboutAppViewModel, AboutAppPage>();
            Register<QRScannerViewModel, QRScannerPage>();
            Register<VerificationCodeViewModel, VerificationCodePage>();
            Register<DeactivationViewModel, DeactivationPage>();
            Register<FaqViewModel, FAQPage>();
            Register<ContactViewModel, ContactPage>();
            Register<CameraPermissionViewModel, CameraPermissionPage>();
            Register<NoLetterReceivedViewModel, NoLetterReceivedPage>();
            Register<ActivationLetterViewModel, ActivationLetterPage>();
            Register<ActivationSmsViewModel, ActivationSMSPage>();
            Register<NoSmsViewModel, NoSMSPage>();
            Register<WidUpgradeStatusViewModel, WIDUpgradeStatusPage>();
            Register<ActivationCompletedViewModel, ActivationCompletedPage>();
            Register<ConfirmRdaViewModel, ConfirmRDAPage>();
            Register<ConfirmViewModel, ConfirmPage>();
            Register<ActivationConfirmViewModel, ActivationConfirmPage>();
            Register<NoDigiDViewModel, NoDigiDPage>();
            Register<KillSwitchViewModel, KillSwitchPage>();
            Register<MoreLoginInformationViewModel, MoreLoginInformationPage>();
            Register<ActivationByAppViewModel, ActivationByAppPage>();
            Register<VerificationCodeEntryViewModel, VerificationCodeEntryPage>();
            Register<ActivationRdaCompletedViewModel, ActivationRdaCompletedPage>();
            Register<ActivationRequestStationViewModel, ActivationRequestStationPage>();
            Register<ToManyActiveAppsViewModel, ToManyActiveAppsPage>();
            Register<OpenSourceLibraryViewModel, OpenSourceLibraryPage>();
            Register<RdaScanFailedViewModel, RDAScanFailedPage>();
            Register<ActivationRdaConfirmViewModel, ActivationRdaConfirmPage>();
            Register<EmailRegisterViewModel, EmailRegisterPage>();
            Register<EmailConfirmViewModel, EmailConfirmPage>();
            Register<HelpInfoViewModel, HelpInfoPage>();
            Register<ConfirmChangePinViewModel, ConfirmChangePINPage>();
            Register<IdCheckNoNfcViewModel, IdCheckNoNfcPage>();
            Register<MessageCenterViewModel, MessageCenterPage>();
            Register<SettingsAppThemeViewModel, SettingsAppThemePage>();
            Register<SettingsLanguageViewModel, SettingsLanguagePage>();
            Register<AP103ViewModel, AP103>();
            Register<AP107ViewModel, AP107>();
            Register<AP109ViewModel, AP109>();
            Register<AP079ViewModel, AP079>();
            Register<AP082ViewModel, AP082>();
            Register<WidPhotoViewModel, WIDPhotoPage>();
            Register<ActivationRdaFailedViewModel, ActivationRdaFailedPage>();
            Register<AP106ViewModel, AP106>();
            Register<AP099ViewModel, AP099>();
            Register<WidSuspendedViewModel, WidSuspendedPage>();
            Register<WhatsNewTourViewModel, WhatsNewTourPage>();
            Register<AP116ViewModel, AP116>();
            Register<UsageHistoryViewModel, UsageHistoryPage>();
            Register<AP117ViewModel, AP117>();
            Register<MainMenuViewModel, MainMenuPage>();
            Register<WebViewViewModel, WebViewPage>();
            Register<VideoPlayerViewModel, VideoPlayerPage>();
            Register<AP118ViewModel, AP118>();
            Register<AP119ViewModel, AP119>();
            Register<AP120ViewModel, AP120>();
            Register<AP087ViewModel, AP087>();
        }

        private void Register<TViewModel, TPage>()
            where TViewModel : CommonBaseViewModel
            where TPage : ContentPage
        {
            if (!_pages.ContainsKey(typeof(TViewModel)))
                _pages.Add(typeof(TViewModel), typeof(TPage));
        }

        private ContentPage InstantiateView(IViewModel viewModel)
        {
            // Figure out what type the view model is
            var viewModelType = viewModel.GetType();

            // look up what type of view it corresponds to
            var viewType = _pages[viewModelType];

            // instantiate it
            var view = (ContentPage)Activator.CreateInstance(viewType);
            view.BindingContext = viewModel;

            CurrentPageId = viewModel.PageId;

            return view;
        }

        private static Page CurrentModalPage => Application.Current.MainPage.Navigation.ModalStack.Count > 0 ? Application.Current.MainPage.Navigation.ModalStack.LastOrDefault() : null;

        public Page GetPage(CommonBaseViewModel viewModel)
        {
            return InstantiateView(viewModel);
        }

        public void SetPage(CommonBaseViewModel viewModel)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var view = InstantiateView(viewModel);
                Application.Current.MainPage = new CustomNavigationPage(view);
                await DependencyService.Get<IA11YService>().NotifyForNewPage();
            });
        }

        public async Task PushAsync(CommonBaseViewModel viewModel, bool animate = true)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var view = InstantiateView(viewModel);

                if (CurrentModalPage is CustomNavigationPage navPage)
                {
                    try
                    {
                        await navPage.Navigation.PushAsync(view, animate);
                    }
                    catch (Exception e)
                    {
                        var dict = new Dictionary<string, string>
                        {
                            {"PageId", CurrentPageId},
                            {"ViewModel", viewModel.GetType().ToString()}
                        };

                        Crashes.TrackError(e, dict);
                    }
                }
                else
                {
                    if (Application.Current.MainPage == null)
                    {
                        Application.Current.MainPage = new CustomNavigationPage(view);
                    }
                    else
                    {
                        var mainPageType = Application.Current.MainPage.GetType();
                        if (mainPageType == typeof(CustomNavigationPage))
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(view, animate);
                        }
                        else
                        {
                            Application.Current.MainPage = new CustomNavigationPage(view);
                        }
                    }
                }
                if (!(viewModel is RdaViewModel)) // Zinvolle mededeling wordt afgebroken, dit is niet handig in dit geval
                    await DependencyService.Get<IA11YService>().NotifyForNewPage();

            });
        }

        public async Task<bool> ConfirmAsync(string confirmAction, RegisterEmailModel model = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            var vm = new ConfirmViewModel(new ConfirmModel(confirmAction)
            {
                RegisterEmailModel = model
            }, false, async result =>
            {
                await Task.Delay(0);
                tcs.TrySetResult(result);
            });

            await PushAsync(vm, false);
            return await tcs.Task;
        }

        public async Task PushModalAsync(CommonBaseViewModel viewModel, bool animate = true, bool nav = true)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var page = InstantiateView(viewModel);

                if (nav)
                {
                    if (CurrentModalPage != null)
                        await CurrentModalPage.Navigation.PushModalAsync(new CustomNavigationPage(page), animate);
                    else
                        await Application.Current.MainPage.Navigation.PushModalAsync(new CustomNavigationPage(page),
                            animate);
                }
                else
                {
                    if (CurrentModalPage != null)
                        await CurrentModalPage.Navigation.PushModalAsync(page, animate);
                    else
                        await Application.Current.MainPage.Navigation.PushModalAsync(page, animate);
                }
            });
        }

        public async Task GoBack(bool animate = true)
        {
            if (CurrentModalPage != null)
                await CurrentModalPage.Navigation.PopAsync(animate);
            else
            {
                if (Application.Current.MainPage is NavigationPage navigationPage)
                    await navigationPage.PopAsync(animate);
                else
                {
                    if (Application.Current.MainPage.Navigation.NavigationStack?.Count >= 1)
                        await Application.Current.MainPage.Navigation.PopAsync(animate);
                    else
                        await DependencyService.Get<INavigationService>().PopToRoot();
                }
            }
        }

        public async Task PopCurrentModalPage(bool animate = true)
        {
            if (CurrentModalPage != null)
            {
                await CurrentModalPage.Navigation.PopModalAsync(animate);
            }
        }

        public async Task ShowMessagePage(MessagePageType pageType, object data = null)
        {
            await PushAsync(new MessageViewModel(pageType, data));
            await DependencyService.Get<IA11YService>().NotifyForNewPage();
        }

        public Task GoToPincodePage(PinCodeModel model, bool animate = true)
        {
            SetPage(new PinCodeViewModel(model));
            DependencyService.Get<IA11YService>().NotifyForNewPage();
            return Task.CompletedTask;
        }

        public async Task PopToRoot(bool force = false)
        {
#if !A11YTEST
            await DependencyService.Get<IA11YService>().NotifyForNewPage();
            App.ClearSession();

            if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.NotActivated)
            {
                if (force)
                    SetPage(new LandingViewModel());
                else
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
            else
                await SessionHelper.StartSession(async () =>
                {
                    if (force)
                        SetPage(new LandingViewModel());
                    else
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                });
#endif
        }

        public string CurrentPageId { get; private set; }

        public async Task<T> OpenPopup<T>(BasePopup<T> popup)
        {
            popup.IsOpened = true;
            return await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }

        public void OpenPopup(BasePopup popup)
        {
            popup.IsOpened = true;
            Application.Current.MainPage.Navigation.ShowPopup(popup);
        }

        public async Task GoToNFCScannerPage(NfcScannerModel model)
        {
            await PushAsync(new WidScannerViewModel(model));
            await DependencyService.Get<IA11YService>().NotifyForNewPage();
        }

        public async Task ResetMainPage(params CommonBaseViewModel[] viewModels)
        {
            var navPage = new CustomNavigationPage(GetPage(new LandingViewModel()));
            
            foreach (var vm in viewModels)
                await navPage.PushAsync(GetPage(vm), false);

            Application.Current.MainPage = navPage;
        }
    }
}
