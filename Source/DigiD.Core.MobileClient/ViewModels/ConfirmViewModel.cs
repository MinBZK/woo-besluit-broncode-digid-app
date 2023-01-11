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
using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.EID.Helpers;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.NFC.Enums;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using DigiD.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class ConfirmViewModel : BaseConfirmViewModel
    {
        public bool ShowPossibleIDs => Model.Action == AuthenticationActions.RDAUpgrade;

#if A11YTEST
        public ConfirmViewModel() : this(new ConfirmModel(AuthenticationActions.ActivateSMSAuthentication), true) { }
#endif

        private async Task Finish(Func<bool, Task> resultAction)
        {
            switch (Model.Action)
            {
                case ConfirmActions.IDCheckPostpone:
                    await NavigationService.PopToRoot();
                    break;
                case ConfirmActions.IDCheckPostponeSMSActivation:
                    await ActivationHelper.AskPushNotificationPermissionAsync(ActivationMethod.SMS);
                    break;
                case ConfirmActions.IDCheckPostponeAppActivation:
                    if (!HttpSession.IsApp2AppSession && !HttpSession.IsWeb2AppSession)
                        HttpSession.TempSessionData = null;

                    await ActivationHelper.CompleteActivation();
                    break;
                case ConfirmActions.IDCheckPostponeAuthentication:
                    await resultAction.Invoke(true);
                    break;
            }
        }

        public ConfirmViewModel(ConfirmModel model, bool canGoBack, Func<bool, Task> resultAction = null) : base(model)
        {
            HasBackButton = canGoBack;

            switch (model.Action)
            {
                case ConfirmActions.ConfirmExistingEmailAddress:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP114";
                    HeaderText = AppResources.AP114_Header;
                    FooterText = string.Format(AppResources.AP114_Message, model.ConfirmData);
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ImageSource = null;
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;

                        CanExecute = false;
                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;

                        CanExecute = false;
                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case ConfirmActions.RequestAccountExistingApp:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP084";
                    HeaderText = AppResources.AP084Header;
                    FooterText = AppResources.AP084Message;
                    CancelText = AppResources.AP084Button2;
                    ButtonText = AppResources.AP084Button1;
                    ImageSource = null;
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case ConfirmActions.RequestAccountExistingAccount:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP083";
                    HeaderText = AppResources.AP083Header;
                    FooterText = AppResources.AP083Message;
                    CancelText = AppResources.AP083Button2;
                    ButtonText = AppResources.AP083Button1;
                    ImageSource = null;
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case ConfirmActions.RdaNoDocumentsFound:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP098";
                    HeaderText = AppResources.AP098_Header;
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_geldig_NL_pp_of_idkaart.svg";
                    break;
                case ConfirmActions.ConfirmEmailAddress:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP112";
                    HeaderText = AppResources.AP112_HeaderText;
                    FooterText = string.Format(AppResources.AP112_Message, model.ConfirmData);
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ImageSource = "resource://DigiD.Resources.afbeelding_email_controle_vraag.svg";
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case ConfirmActions.RegisterEmailAddress:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP101";
                    HeaderText = AppResources.AP101_Header;
                    FooterText = !string.IsNullOrEmpty(HttpSession.WebService) ? string.Format(AppResources.AP101_MessageAddition, HttpSession.WebService) : AppResources.AP101_Message;
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ImageSource = "resource://DigiD.Resources.afbeelding_email_toevoegen.svg";

                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await DependencyService.Get<IEmailService>().Register(null);
                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case AuthenticationActions.ConfirmWebserviceUpgrade:
                    {
                        if (resultAction == null)
                            throw new ArgumentNullException(nameof(resultAction));

                        PageId = "AP070";
                        HeaderText = AppResources.AP070_Header;
                        FooterText = string.Format(AppResources.AP070_Message, model.SessionInfo.NewAuthenticationLevelStartDate!.Value.ToString("dd-MM-yyyy"));
                        CancelText = AppResources.No;
                        ButtonText = AppResources.Yes;
                        CancelCommand = new AsyncCommand(async () =>
                        {
                            await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.IDCheckPostponeAuthentication), true, resultAction));
                        });

                        ButtonCommand = new AsyncCommand(async () =>
                        {
                            await NavigationService.PushAsync(new ConfirmRdaViewModel(async result =>
                            {
                                if (!result)
                                    await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.IDCheckPostponeAuthentication), false, resultAction));
                                else
                                    await AuthenticationHelper.ContinueRdaUpgradeWhileAuthenticate(null, resultAction);
                            }));
                        });
                        break;
                    }
                case AuthenticationActions.ConfirmWebserviceUpgradeNoNfc:
                    {
                        if (resultAction == null)
                            throw new ArgumentNullException(nameof(resultAction));

                        PageId = "AP102";
                        HeaderText = AppResources.AP102_Header;
                        FooterText = string.Format(AppResources.AP102_Message, model.SessionInfo.NewAuthenticationLevelStartDate!.Value.ToString("dd-MM-yyyy"));
                        ButtonText = AppResources.ActivationCompletedAction;
                        CancelText = string.Empty;
                        ButtonCommand = new AsyncCommand(async () =>
                        {
                            await resultAction.Invoke(true);
                        });
                        break;
                    }
                case ConfirmActions.RequestPushNotificationPermission:
                    PageId = "AP021";
                    FooterText = AppResources.AP021_Message;
                    HeaderText = AppResources.AP021_Header;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_notificatie.svg";
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;

                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;
                        CanExecute = false;

                        await RemoteNotificationHelper.RequestToken(true);
                        await resultAction.Invoke(true);
                        CanExecute = true;
                    }, () => CanExecute);

                    CancelCommand = new AsyncCommand(async () =>
                    {
                        if (!CanExecute)
                            return;

                        CanExecute = false;
                        await RemoteNotificationHelper.RequestToken(false);
                        await resultAction.Invoke(false);
                        CanExecute = true;
                    }, () => CanExecute);
                    break;
                case AuthenticationActions.DeactivateApp:
                    PageId = "AP076";
                    HeaderText = AppResources.AP076_Header;
                    FooterText = AppResources.AP076_Message;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_digid_app_deactiveren.svg";
                    break;
                case AuthenticationActions.CANCEL_ACCOUNT:
                    PageId = "AP020";
                    HeaderText = AppResources.AP023_Header;
                    FooterText = AppResources.AP023_Message;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_opheffen_digid_account.svg";
                    break;
                case ConfirmActions.ActivateViaRequestStationStart:
                    PageId = "AP057";
                    HeaderText = AppResources.AP057_Header;
                    FooterText = AppResources.AP057_Message;
                    ButtonText = AppResources.AP057_ButtonConfirmText;
                    CancelText = AppResources.AP057_ButtonCancelText;
                    CancelCommand = new AsyncCommand(async () =>
                        {
                            await NavigationService.ShowMessagePage(MessagePageType.ActivateViaRequestStationCancel);
                        });
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        AppSession.Process = Process.AppActivationViaRequestStation;
                        await NavigationService.PushAsync(new ActivationLoginViewModel());
                    });
                    NavCloseCommand = new AsyncCommand(async () => { await NavigationService.PopToRoot(); });
                    break;
                case ConfirmActions.IDCheckPostpone:
                case ConfirmActions.IDCheckPostponeSMSActivation:
                case ConfirmActions.IDCheckPostponeAppActivation:
                case ConfirmActions.IDCheckPostponeAuthentication:
                    if (Model.Action == ConfirmActions.IDCheckPostponeAuthentication && resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP062";
                    HeaderText = AppResources.AP096_Header;
                    FooterText = AppResources.AP096_Message;
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_idcheck_uitstellen.svg";
                    CancelCommand = new AsyncCommand(async () => { await Finish(resultAction); });
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        var date = DateTime.UtcNow;
                        var evening = date.Hour >= 19 || date.Hour < 1;
                        var result = await Application.Current.MainPage.DisplayActionSheet(AppResources.M07_Title, AppResources.M07_Cancel, null, AppResources.M07_Option1, evening ? AppResources.M07_Option3 : AppResources.M07_Option2, evening ? AppResources.M07_Option4 : AppResources.M07_Option3);

                        var dt = date;

                        if (result == AppResources.M07_Cancel)
                        {
                            await Finish(resultAction);
                            return;
                        }

                        if (result == AppResources.M07_Option1)
#if PROD || PREPROD
                            dt = date.AddHours(1);
#else
                            dt = date.AddMinutes(1);
#endif
                        else if (result == AppResources.M07_Option2)
                            dt = date.Date.AddHours(19);
                        else if (result == AppResources.M07_Option3)
                            dt = date.Date.AddDays(1).AddHours(9);
                        else if (result == AppResources.M07_Option4)
                            dt = date.Date.AddDays(1).AddHours(9);

                        await LocalNotificationHelper.SetNotification(dt.ToUniversalTime(), NotificationType.IDcheck);
                        await Finish(resultAction);
                    });
                    break;
                case ConfirmActions.IDcheckStart:
                    PageId = "AP061";
                    HeaderText = AppResources.AP061_Header;
                    FooterText = AppResources.AP061_Message;
                    CancelText = AppResources.No;
                    ButtonText = AppResources.Yes;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        await RdaHelper.Init(true);
                        DialogService.HideProgressDialog();
                    });
                    break;
                case ConfirmActions.IDcheckStartFromMenu:
                    PageId = "AP090";
                    HeaderText = AppResources.AP090_Header;
                    FooterText = AppResources.AP090_Message;
                    CancelText = string.Empty;
                    ButtonText = AppResources.AP090_Button;
                    HasBackButton = true;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        await RdaHelper.Init(true);
                        DialogService.HideProgressDialog();
                    });
                    break;
                case ConfirmActions.UpgradeToSubWhileSub:
                    HeaderText = AppResources.UpgradeToSubWhileSubHeader;
                    FooterText = AppResources.UpgradeToSubWhileSubMessage;
                    ButtonText = AppResources.UpgradeToSubWhileSubButtonText;
                    CancelText = string.Empty;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        await RdaHelper.Init(true);
                        DialogService.HideProgressDialog();
                    });
                    NavCloseCommand = CancelCommand;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_document_idcheck_al_uitgevoerd.svg";
                    PageId = "AP008";
                    break;
                case AuthenticationActions.RDAUpgrade:
                    if (resultAction == null)
                        throw new ArgumentNullException(nameof(resultAction));

                    PageId = "AP072";
                    HeaderText = AppResources.IDCheckerConfirmHeader;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_document_controle.svg";
                    FooterText = AppResources.ConfirmUpgradeWithWIDCheckerMessage;
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        await resultAction.Invoke(true);
                    });
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        await resultAction.Invoke(false);
                    });
                    ButtonText = AppResources.GoNextButtonText;
                    break;
                case AuthenticationActions.ACTIVATE_WITH_APP:
                    PageId = "AP056";
                    HeaderText = AppResources.Confirm_ACTIVATE_WITH_APP_Header;
                    FooterText = AppResources.Confirm_ACTIVATE_WITH_APP_Footer;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_activeer_andere_digid_app.svg";
                    break;
                case AuthenticationActions.EMAIL_CHANGE:
                    HeaderText = AppResources.Confirm_EMAIL_CHANGE_Header;
                    FooterText = AppResources.Confirm_EMAIL_CHANGE_Message;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.afbeelding_email_wijzigen.svg";
                    PageId = "AP026";
                    break;
                case AuthenticationActions.EMAIL_ADD:
                    HeaderText = AppResources.Confirm_EMAIL_ADD_Header;
                    FooterText = AppResources.Confirm_EMAIL_ADD_Message;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.afbeelding_email_toevoegen.svg";
                    PageId = "AP027";
                    break;
                case AuthenticationActions.EMAIL_REMOVE:
                    HeaderText = AppResources.Confirm_EMAIL_REMOVE_Header;
                    FooterText = AppResources.Confirm_EMAIL_REMOVE_Message;
                    CancelText = AppResources.Cancel;
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    ImageSource = "resource://DigiD.Resources.afbeelding_email_deactiveren.svg";
                    PageId = "AP028";
                    break;
                case AuthenticationActions.LetterNotification:
                    HeaderText = AppResources.ActivationHeader;
                    FooterText = AppResources.ActivationLetterNotificationFooter;
                    ButtonText = AppResources.ActivationLetterNotificationNotifyButton;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_brief_herinnering.svg";
                    CancelText = AppResources.ActivationLetterNotificaitonSkipButton;

                    PageId = "AP069";
                    ButtonCommand = new AsyncCommand(async () =>
                    {
                        await LetterNotificationHelper.CreateLetterNotification();
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationPending);
                    });
                    CancelCommand = new AsyncCommand(async () =>
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationPending);
                    });
                    break;
                case AuthenticationActions.ActivateSMSAuthentication:
                    PageId = "AP034";
                    FooterText = AppResources.ActivateSMSAuthenticationHeader;
                    HeaderText = AppResources.ActivateSMSAuthenticationMessage;
                    ButtonText = AppResources.ConfirmOK;
                    ImageSource = "resource://DigiD.Resources.afbeelding_sms_activeren.svg";
                    ButtonCommand = ConfirmCommand;
                    break;
                case AuthenticationActions.ChangePassword:
                    PageId = "AP024";
                    FooterText = AppResources.ChangePasswordFooter;
                    HeaderText = AppResources.ChangePasswordHeader;
                    ButtonText = AppResources.ConfirmOK;
                    ImageSource = "resource://DigiD.Resources.afbeelding_wijzigen_wachtwoord.svg";
                    ButtonCommand = ConfirmCommand;
                    break;
                case AuthenticationActions.ChangePhoneNumber:
                    PageId = "AP025";
                    FooterText = AppResources.ChangePhoneNumberFooter;
                    HeaderText = AppResources.ChangePhoneNumberHeader;
                    ImageSource = "resource://DigiD.Resources.afbeelding_wijzigen_telefoon.svg";
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    break;
                case AuthenticationActions.ActivationByLetter:
                    PageId = "AP012";
                    HeaderText = AppResources.ActivationConfirmHeader;
                    FooterText = AppResources.ActivationConfirmFooter;
                    ImageSource = "resource://DigiD.Resources.afbeelding_activeringscode_brief.svg";
                    ButtonText = AppResources.ActivationConfirmButton;
                    ButtonCommand = ConfirmCommand;
                    break;
                case AuthenticationActions.REVOKE_DRIVING_LICENSE:
                    PageId = "AP415";
                    HeaderText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDRetractConfirmHeader, DocumentType.DrivingLicense.Translate().ToLowerInvariant());
                    FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDRetractConfirmMessage, DocumentType.DrivingLicense.Translate().ToLowerInvariant());
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_failed.svg?assembly=DigiD.Common";
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    CardHelper.SetCard(DocumentType.DrivingLicense);
                    break;
                case AuthenticationActions.REVOKE_IDENTITY_CARD:
                    PageId = "AP416";
                    HeaderText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDRetractConfirmHeader, DocumentType.IDCard.Translate().ToLowerInvariant());
                    FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDRetractConfirmMessage, DocumentType.IDCard.Translate().ToLowerInvariant());
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_failed.svg?assembly=DigiD.Common";
                    ButtonText = AppResources.ConfirmOK;
                    ButtonCommand = ConfirmCommand;
                    CardHelper.SetCard(DocumentType.IDCard);
                    break;
                case AuthenticationActions.UpgradeWithWIDChecker:
                    PageId = "AP005";
                    HeaderText = AppResources.ConfirmUpgradeWithWIDCheckerHeader;
                    FooterText = AppResources.ConfirmUpgradeWithWIDCheckerMessage;
                    ImageSource = "resource://DigiD.Resources.digid_afbeelding_idcheck_bevestigen.svg";
                    ButtonCommand = ConfirmCommand;
                    ButtonText = AppResources.ConfirmOK;
                    AppSession.Process = Process.IDChecker;
                    break;
            }
        }

        private async Task ResetApp(bool needReset)
        {
            if (needReset)
                App.Reset();

            switch (Model.SessionInfo.Action)
            {
                case AuthenticationActions.CANCEL_ACCOUNT:
                    await NavigationService.ShowMessagePage(MessagePageType.AccountCancelledSuccess);
                    break;
                case AuthenticationActions.DeactivateApp:
                    await NavigationService.ShowMessagePage(MessagePageType.DeactivateAppSuccess);
                    break;
            }
        }

        public AsyncCommand NextCommand { get; set; }
        public bool IsBlurVisible { get; set; }

        public AsyncCommand ConfirmCommand => new AsyncCommand(async () =>
        {
            if (Model.WIDSessionResponse != null)
            {
                await ShowPINForEIDCommand.ExecuteAsync();
                return;
            }

            var confirmModel = new ConfirmRequest
            {
                AppId = DependencyService.Get<IMobileSettings>().AppId,
            };

            var result = await DependencyService.Get<IAuthenticationService>().Confirm(confirmModel);

            if (result.ApiResult == ApiResult.Ok && HttpSession.IsWeb2AppSession)
            {
                IsBlurVisible = true;
                await DependencyService.Get<IA11YService>().Speak(AppResources.LoginSuccessMessage);
                NextCommand = new AsyncCommand(async () =>
                {
                    IsNextCommandExecuted = true;
                    App.ShowSplashScreen(true, true);
                    await WebserviceConfirmViewModel.HandleWeb2AppSuccess(Model.SessionInfo, Model.WIDSessionResponse, result.NeedDeactivation);
                }, () => !IsNextCommandExecuted);

                return;
            }

            switch (Model.SessionInfo.Action)
            {
                case AuthenticationActions.CANCEL_ACCOUNT:
                    await ResetApp(true);
                    break;
                case AuthenticationActions.DeactivateApp:
                    await ResetApp(result.NeedDeactivation);
                    break;
                case AuthenticationActions.RDAUpgrade:
                    await RdaHelper.StartUpgrade();
                    return;
                case AuthenticationActions.ReRequestLetter:
                    await NavigationService.PushAsync(new ActivationLetterViewModel(true));
                    break;
                case AuthenticationActions.UpgradeWithWIDChecker:
                    if (HttpSession.AppSessionId != null)
                        await NavigationService.PushAsync(new WidUpgradeStatusViewModel());
                    break;
                case AuthenticationActions.ChangePassword:
                    await NavigationService.ShowMessagePage(MessagePageType.PasswordChangeSuccess);
                    App.ClearSession();
                    break;
                case AuthenticationActions.ActivateSMSAuthentication:
                    await NavigationService.ShowMessagePage(
                        MessagePageType.ActivateSMSAuthenticationSuccess);
                    App.ClearSession();
                    break;
                case AuthenticationActions.ActivationByLetter:
                    await NavigationService.PushAsync(new ActivationLetterViewModel(false));
                    break;
                case AuthenticationActions.ChangePhoneNumber:
                    await NavigationService.ShowMessagePage(MessagePageType.PhoneNumberChangeSuccess);
                    App.ClearSession();
                    break;
                case AuthenticationActions.REVOKE_DRIVING_LICENSE:
                case AuthenticationActions.REVOKE_IDENTITY_CARD:
                    await NavigationService.ShowMessagePage(MessagePageType.WIDRetractSuccess);
                    App.ClearSession();
                    break;
                case AuthenticationActions.EMAIL_CHANGE:
                    await NavigationService.ShowMessagePage(MessagePageType.EmailChange);
                    App.ClearSession();
                    break;
                case AuthenticationActions.EMAIL_ADD:
                    await NavigationService.ShowMessagePage(MessagePageType.EmailAdd);
                    App.ClearSession();
                    break;
                case AuthenticationActions.EMAIL_REMOVE:
                    await NavigationService.ShowMessagePage(MessagePageType.EmailRemove);
                    App.ClearSession();
                    break;
                case AuthenticationActions.ACTIVATE_WITH_APP:
                    await NavigationService.ShowMessagePage(MessagePageType.AppActivationWithAppSuccess);
                    App.ClearSession();
                    break;
            }
        });

        public override async void Cancel()
        {
            base.Cancel();

            if (Model.SessionInfo != null)
                await App.CancelSession(true);
        }

        public override bool OnBackButtonPressed()
        {
            return !HasBackButton;
        }
    }
}
