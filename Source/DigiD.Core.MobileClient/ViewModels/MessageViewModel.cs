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
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Browser = DigiD.Common.Enums.Browser;
using MessagePageType = DigiD.Common.Enums.MessagePageType;

namespace DigiD.ViewModels
{
    internal class MessageViewModel : BaseMessageViewModel
    {
        protected override async Task ChangePIN()
        {
            await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.ChangeWIDPIN), false));
        }

        private async Task ContinueAfterEmailRegistration(object data)
        {
            if (DependencyService.Get<IMobileSettings>().ActivationStatus != ActivationStatus.Activated)
                await ActivationHelper.ContinueActivation((RegisterEmailModel)data);
            else
            {
                if (!string.IsNullOrEmpty(HttpSession.WebService))
                    await NavigationService.ShowMessagePage(MessagePageType.LoginSuccess);
                else
                    await NavigationService.PopToRoot(true);
            }
        }

#if A11YTEST
        public MessageViewModel() : this(MessagePageType.UpgradeAccountWithNFCSuccess, null) { }
#endif

#pragma warning disable S1479 // "switch" statements should not have too many "case" clauses
        internal MessageViewModel(MessagePageType pageType, object data = null) : base(pageType)
#pragma warning restore S1479 // "switch" statements should not have too many "case" clauses
        {
            switch (pageType)
            {
                case MessagePageType.AccountBlocked:
                    PageId = "AP234";
                    SetData(AppResources.ActivationFailedHeader, string.Format(CultureInfo.InvariantCulture, AppResources.SingleDeviceAccountBlocked, data), AppResources.OK, false);
                    break;
                case MessagePageType.ContactHelpDesk:
                    PageId = "AP287";
                    SetData(AppResources.AP287_Header, AppResources.AP287_Message, AppResources.OK, false);
                    break;
                case MessagePageType.EmailRegistrationSuccessfulVerified:
                    PageId = "AP113";
                    SetData(AppResources.AP113_Header, AppResources.AP113_Message, AppResources.OK, true);
                    break;
                case MessagePageType.ClassifiedDeceased:
                    PageId = "AP283";
                    SetData(AppResources.AP283_Header, AppResources.AP283_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountNotAvailable:
                    PageId = "AP260";
                    SetData(AppResources.AP260_Header, AppResources.AP260_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountDeceased:
                    PageId = "AP261";
                    SetData(AppResources.AP261_Header, AppResources.AP261_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountBlockedTemporarily:
                    PageId = "AP262";
                    SetData(AppResources.AP262_Header, string.Format(AppResources.AP262_Message, (int)data!), AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountTooOftenDay:
                    PageId = "AP263";
                    SetData(AppResources.AP263_Header, AppResources.AP263_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountTooOftenMonth:
                    PageId = "AP264";
                    SetData(AppResources.AP264_Header, string.Format(AppResources.AP264_Message, (int)data!), AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountAccountBlocked:
                    PageId = "AP278";
                    SetData(AppResources.AP278_Header, AppResources.AP278_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountBRPTimeout:
                    PageId = "AP279";
                    SetData(AppResources.AP279_Header, AppResources.AP279_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountNotLatestAddress:
                    PageId = "AP280";
                    SetData(AppResources.AP280_Header, AppResources.AP280_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountAddressUnderInvestigating:
                    PageId = "AP281";
                    SetData(AppResources.AP281_Header, AppResources.AP281_Message, AppResources.OK, false);
                    break;
                case MessagePageType.RequestAccountAddressAbroad:
                    PageId = "AP282";
                    SetData(AppResources.AP282_Header, AppResources.AP282_Message, AppResources.OK, false);
                    break;
                case MessagePageType.NoValidDocuments:
                    PageId = "AP277";
                    SetData("ID-check niet mogelijk", "U kunt alleen een ID-check uitvoeren met een Nederlands rijbewijs uitgegeven na 14 november 2014, identiteitskaart of paspoort. U kunt de ID-check starten vanuit het menu als u zo’n document heeft.", AppResources.OK, false);
                    break;
                case MessagePageType.AppDeactivated:
                    PageId = "AP276";
                    SetData(AppResources.AP276_Header, AppResources.AP276_Message, AppResources.OK, false);
                    break;
                case MessagePageType.EmailRegistrationSuccess:
                    PageId = "AP100";
                    SetData(AppResources.AP100_Header, AppResources.AP100_Message, AppResources.OK, true);
                    break;
                case MessagePageType.EmailRegistrationCodeBlocked:
                    PageId = "AP269";
                    SetData(AppResources.AP269_Header, AppResources.AP269_Message, AppResources.OK, false, new AsyncCommand(
                        async () =>
                        {
                            await ContinueAfterEmailRegistration(data);
                        }));
                    break;
                case MessagePageType.EmailRegistrationTooManyMails:
                    PageId = "AP270";
                    SetData(AppResources.AP270_Header, AppResources.AP270_Message, AppResources.OK, false, new AsyncCommand(
                        async () =>
                        {
                            await ContinueAfterEmailRegistration(data);
                        }));
                    break;
                case MessagePageType.EmailRegistrationMaximumReached:
                    PageId = "AP271";
                    SetData(AppResources.AP271_Header, AppResources.AP271_Message, AppResources.OK, false, new AsyncCommand(
                        async () =>
                        {
                            await ContinueAfterEmailRegistration(data);
                        }));
                    break;
                case MessagePageType.EmailRegistrationAlreadyVerified:
                    PageId = "AP272";
                    SetData(AppResources.AP272_Header, AppResources.AP272_Message, AppResources.OK, true, new AsyncCommand(
                        async () =>
                        {
                            await ContinueAfterEmailRegistration(data);
                        }));
                    break;
                case MessagePageType.ActivationFailedTooManyDevices:
                    {
                        PageId = "AP200";
                        SetData(AppResources.AP200_Header, AppResources.AP200_Message, AppResources.AP200_Button, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.UnknownError:
                    {
                        PageId = "AP252";
                        SetData(MessagePageType.UnknownError);
                        break;
                    }
                case MessagePageType.InAppActivationCodeBlocked:
                    PageId = "AP268";
                    SetData(AppResources.InAppActivationLetterBlockedHeader, AppResources.InAppActivationLetterBlockedMessage, AppResources.OK, false, command: PopToRootCommand);
                    break;
                case MessagePageType.InAppActivationCodeExpired:
                    PageId = "AP267";
                    SetData(AppResources.InAppActivationCodeExpiredHeader, AppResources.InAppActivationCodeExpiredMessage, AppResources.OK, false, command: PopToRootCommand);
                    break;
                case MessagePageType.DeactivateAppSuccess:
                    PageId = "AP077";
                    SetData(AppResources.AP077_Header, AppResources.AP077_Message, AppResources.OK, true);
                    break;
                case MessagePageType.ActivateViaRequestStationCodeExpired:
                    PageId = "AP258";
                    SetData(AppResources.ActivateViaRequestStationCodeExpiredHeader, AppResources.ActivateViaRequestStationCodeExpiredMessage, AppResources.OK, false);
                    break;
                case MessagePageType.DeviceNotSupported:
                    PageId = "AP257";
                    SetData(AppResources.DeviceNotSupportedHeader, AppResources.DeviceNotSupportedMessage, AppResources.OK, false, new Command(
                        () =>
                        {
                            //Do nothing :-)
                        }));
                    break;
                case MessagePageType.ChangePinSuccess:
                    PageId = "AP075";
                    SetData(AppResources.ChangePinSuccessHeader, AppResources.ChangePinSuccessMessage, AppResources.OK, true);
                    break;
                case MessagePageType.ChangePinFailed:
                    PageId = "AP254";
                    SetData(AppResources.ChangePinFailedHeader, AppResources.ChangePinFailedMessage, AppResources.OK, false);
                    break;
                case MessagePageType.ChangePinMaxReached:
                    PageId = "AP074";
                    SetData(AppResources.ChangePinMaxReachedHeader, string.Format(CultureInfo.InvariantCulture, AppResources.ChangePinMaxReachedMessage, App.Configuration.MaxPinChangePerDay), AppResources.OK, null);
                    break;
                case MessagePageType.AccountCancelledSuccess:
                    PageId = "AP023";
                    SetData(AppResources.AP024_Header, AppResources.AP024_Message, AppResources.OK, true);
                    break;
                case MessagePageType.ActivateViaRequestStationCancel:
                    PageId = "AP058";
                    SetData(AppResources.AP058_Header, AppResources.AP058_Message, AppResources.OK, null);
                    break;
                case MessagePageType.AppActivationWithAppSuccess:
                    PageId = "AP055";
                    SetData(AppResources.AP85_Header, AppResources.AP85_Message, AppResources.OK, true, PopToRootCommand);
                    break;
                case MessagePageType.AuthenticationAborted:
                    PageId = "AP251";
                    SetData(AppResources.AuthenticationAbortedHeader, AppResources.AuthenticationAbortedMessage, AppResources.OK, false, new AsyncCommand(async () =>
                    {
                        if (data is AuthenticateChallengeResponse response)
                        {
                            if (!string.IsNullOrEmpty(response.ReturnURL))
                            {
                                if (Device.RuntimePlatform == Device.iOS)
                                {
                                    var returnUrl = response.ReturnURL;

                                    switch (HttpSession.Browser)
                                    {
                                        case Browser.Chrome:
                                            returnUrl = $"googlechromes://{returnUrl.Replace("https://", "")}";
                                            break;
                                        case Browser.Firefox:
                                            returnUrl = $"firefox://open-returnUrl?returnUrl={WebUtility.UrlEncode(returnUrl)}";
                                            break;
                                    }

                                    await Launcher.OpenAsync(returnUrl);
                                }
                                else
                                    DependencyService.Get<IDevice>().CloseApp();
                            }

                            await NavigationService.PopToRoot();
                        }
                    }));
                    break;
                case MessagePageType.NoBSN:
                    PageId = "AP208";
                    SetData(AppResources.ActivationFailedHeader, AppResources.SingleDeviceNoBSNOnAccount, AppResources.OK, false);
                    break;
                case MessagePageType.EmailChange:
                    PageId = "AP031";
                    SetData(AppResources.EMAIL_CHANGE_SUCCESS_HEADER, AppResources.EMAIL_CHANGE_SUCCESS_MESSAGE, AppResources.OK, true);
                    break;
                case MessagePageType.EmailAdd:
                    SetData(AppResources.EMAIL_ADD_SUCCESS_HEADER, AppResources.EMAIL_ADD_SUCCESS_MESSAGE, AppResources.OK, true);
                    PageId = "AP032";
                    break;
                case MessagePageType.EmailRemove:
                    SetData(AppResources.EMAIL_REMOVE_SUCCESS_HEADER, AppResources.EMAIL_REMOVE_SUCCESS_MESSAGE, AppResources.OK, true);
                    PageId = "AP033";
                    break;
                case MessagePageType.UpgradeAccountWithIDCheckerSuccess:
                    {
                        PageId = "AP040";
                        SetData(AppResources.UpgradeAccountWithIDCheckerSuccessHeader, AppResources.UpgradeAccountWithIDCheckerSuccessMessage, AppResources.OK, true);
                        break;
                    }
                case MessagePageType.RDADisabled:
                    {
                        PageId = "AP226";
                        SetData(AppResources.RDADisabledHeader, AppResources.RDADisabledMessage, AppResources.OK, false);
                        break;
                    }
                case MessagePageType.LoginSuccess:
                    {
                        if (AppSession.Process == Process.UpgradeAndAuthenticate)
                        {
                            PageId = "AP067";
                            SetData(AppResources.UpgradeAndAuthenticateSuccesHeader, AppResources.UpgradeAndAuthenticateSuccesMessage, AppResources.OK, true, new AsyncCommand(
                                async () =>
                                {
                                    switch (data)
                                    {
                                        case BaseResponse _:
                                            await App2AppHelper.ReturnToClientApp();
                                            break;
                                        case string url:
                                            await AuthenticationHelper.ProcessWebAuthentication(url);
                                            return;
                                        default:
                                            await NavigationService.PopToRoot();
                                            break;
                                    }
                                }));
                        }
                        else
                        {
                            if (HttpSession.IsApp2AppSession)
                                PageId = "AP017a";
                            else if (HttpSession.IsWeb2AppSession)
                                PageId = "AP017b";
                            else
                                PageId = "AP017";

                            SetData(AppResources.LoginSuccessHeader, AppResources.LoginSuccessMessage, AppResources.LoginSuccessAction, true);
                        }
                        break;
                    }
                case MessagePageType.AppBlocked:
                    PageId = "AP236";
                    if (HttpSession.IsApp2AppSession)
                    {
                        SetData(AppResources.AppBlockedHeader, AppResources.AppBlockedMessage, AppResources.OK, false, new AsyncCommand(async () =>
                        {
                            DialogService.HideProgressDialog();
                            await App2AppHelper.ReturnToClientApp();
                        }));
                    }
                    else
                        SetData(AppResources.AppBlockedHeader, AppResources.AppBlockedMessage, AppResources.OK, false, new AsyncCommand(async () => await NavigationService.PopToRoot(true)));
                    break;
                case MessagePageType.ActivateSMSAuthenticationSuccess:
                    PageId = "AP035";
                    SetData(AppResources.ActivateSMSAuthenticationSuccessHeader, AppResources.ActivateSMSAuthenticationSuccessMessage, AppResources.OK, true);
                    break;
                case MessagePageType.ActivateSMSAuthenticationBlocked:
                    PageId = "AP249";
                    SetData(AppResources.ActivateSMSAuthenticationBlockedHeader, AppResources.ActivateSMSAuthenticationBlockedMessage, AppResources.OK, false);
                    break;
                case MessagePageType.ActivationFailedTooSoon:
                    if (AppSession.Process == Process.AppActivationViaRequestStation)
                    {
                        PageId = "AP205";
                        SetData(AppResources.ActivationFailedTooSoonHeader, AppResources.ActivationFailedTooSoonMessage, AppResources.OK, false);
                    }
                    else
                    {
                        PageId = "AP284";
                        SetData(AppResources.AP284_Header, AppResources.AP284_Message, AppResources.OK, false);
                    }
                    break;
                case MessagePageType.ActivationFailedTooFast:
                    if (AppSession.Process == Process.AppActivationViaRequestStation)
                    {
                        PageId = "AP206";
                        SetData(AppResources.ActivationFailedHeader, string.Format(CultureInfo.InvariantCulture, AppResources.ActivationFailedTooFastMessage, Payload.blokkering_digid_app_aanvragen, ((DateTimeOffset)Payload.next_registration_date).ToString("dd-MM-yyyy")), AppResources.OK, false);
                    }
                    else
                    {
                        PageId = "AP285";
                        SetData(AppResources.AP285_Header, string.Format(CultureInfo.InvariantCulture, AppResources.AP285_Message, ((DateTimeOffset)Payload.next_registration_date).ToString("dd-MM-yyyy")), AppResources.OK, false);
                    }
                    break;
                case MessagePageType.ReRequestLetterSuccess:
                    PageId = "AP071";
                    SetData(AppResources.ActivationHeader, AppResources.ReRequestLetterSuccessMessage, AppResources.OK, true);
                    ImageSource = "resource://DigiD.Resources.afbeelding_wachten_op_brief.svg";
                    break;
                case MessagePageType.GBAFailed:
                    PageId = "AP248";
                    SetData(AppResources.ActivationFailedHeader, AppResources.GBAFailedMessage, AppResources.OK, false);
                    break;
                case MessagePageType.ActivationTimeElapsed:
                    {
                        PageId = "AP202";
                        SetData(AppResources.ActivationTimeElapsedHeader, AppResources.ActivationTimeElapsedMessage, AppResources.ActivationTimeElapsedAction, false);
                        break;
                    }
                case MessagePageType.ActivationPending:
                    {
                        PageId = "AP011";
                        SetData(AppResources.ActivationHeader, AppResources.ActivationPendingMessage, AppResources.ActivationPendingButton, true, new AsyncCommand(
                            async () =>
                            {
                                if (HttpSession.IsApp2AppSession)
                                {
                                    HttpSession.TempSessionData.SwitchSession();
                                    await DependencyService.Get<IAuthenticationService>().AbortAuthentication("app_is_pending");
                                    await App2AppHelper.ReturnToClientApp();
                                }
                                else
                                    PopToRootCommand.Execute(null);
                            }));
                        ImageSource = "resource://DigiD.Resources.afbeelding_wachten_op_brief.svg";
                        break;
                    }
                case MessagePageType.ActivationDisabled:
                    {
                        PageId = "AP203";
                        SetData(AppResources.ActivationDisabledHeader, AppResources.ActivationDisabledMessage, AppResources.ActivationDisabledAction, false);
                        break;
                    }
                case MessagePageType.ActivationCancelled:
                    {
                        PageId = "AP009";
                        SetData(AppResources.ActivationCancelledHeader, string.Empty, AppResources.ActivationCancelledAction, null, new Command(
                            () =>
                            {
                                if (DemoHelper.CurrentUser != null)
                                    DependencyService.Get<IDemoSettings>().Reset();
                                PopToRootCommand.Execute(null);
                            }));
                        break;
                    }
                case MessagePageType.ActivationLetterBlocked:
                    {
                        PageId = "AP253";
                        SetData(AppResources.ActivationLetterBlockedHeader, AppResources.ActivationLetterBlockedMessage, AppResources.ActivationLetterBlockedAction, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.ActivationLetterExpired:
                    {
                        PageId = "AP204";
                        SetData(AppResources.ActivationLetterExpiredHeader, AppResources.ActivationLetterExpiredMessage, AppResources.OK, false, command: PopToRootCommand);
                        break;
                    }
                case MessagePageType.ActivationFailed:
                    {
                        PageId = "AP201";
                        SetData(AppResources.ActivationFailedHeader, AppResources.ActivationFailedMessage, AppResources.ActivationFailedAction, false);
                        break;
                    }
                case MessagePageType.LoginUnknown:
                    {
                        PageId = "AP250";
                        SetData(AppResources.LoginUnknownHeader, AppResources.LoginUnknownMessage, AppResources.LoginUnknownAction, false);
                        break;
                    }
                case MessagePageType.LoginAborted:
                    {
                        PageId = "AP216";
                        SetData(AppResources.LoginAbortedHeader, AppResources.LoginAbortedMessage, AppResources.LoginAbortedAction, false);
                        break;
                    }
                case MessagePageType.NoInternetConnection:
                    {
                        PageId = "AP218";
                        var action = new AsyncCommand(async () => await NavigationService.PopToRoot());
                        SetData(AppResources.NoInternetConnectionHeader, AppResources.NoInternetConnectionMessage, AppResources.NoInternetConnectionAction, false, action);
                        break;
                    }
                case MessagePageType.LoginCancelled:
                    {
                        PageId = "AP018";
                        SetData(AppResources.LoginCancelledHeader, AppResources.LoginCancelledMessage, AppResources.LoginCancelledAction, null);
                        return;
                    }
                case MessagePageType.BrowserUnsupported:
                    {
                        PageId = "AP224";
                        SetData(AppResources.BrowserUnsupportedHeader, AppResources.BrowserUnsupportedMessage, AppResources.BrowserUnsupportedAction, false);
                        break;
                    }
                case MessagePageType.NoNFCSupport:
                    {
                        PageId = "AP230";
                        SetData(AppResources.NoNFCHeader, AppResources.NoNFCMessage, AppResources.NoNFCAction, false);
                        break;
                    }
                case MessagePageType.InsufficientLoginLevel:
                    {
                        PageId = "AP247";
                        SetData(MessagePageType.InsufficientLoginLevel);
                        break;
                    }
                case MessagePageType.UpgradeAccountWithNFCSuccess:
                    {
                        PageId = "AP039";
                        SetData(AppResources.UpgradeAccountWithNFCSuccessHeader, AppResources.UpgradeAccountWithNFCSuccessMessage, AppResources.UpgradeAccountWithNFCSuccessAction
                            , true);
                        break;
                    }
                case MessagePageType.UpgradeAccountWithNFCFailed:
                    {
                        PageId = "AP229";
                        SetData(AppResources.UpgradeAccountWithNFCFailedHeader, AppResources.UpgradeAccountWithNFCFailedMessage, AppResources.OK, false);
                        break;
                    }
                case MessagePageType.UpgradeAccountWithNFCError:
                    {
                        PageId = "AP228";
                        SetData(AppResources.UpgradeAccountWithNFCErrorHeader, AppResources.UpgradeAccountWithNFCErrorMessage, AppResources.UpgradeAccountWithNFCErrorAction, false);
                        break;
                    }
                case MessagePageType.UpgradeAccountWithNFCCancelled:
                    {
                        PageId = "AP041";
                        SetData(AppResources.UpgradeAccountWithNFCCancelledHeader, AppResources.UpgradeAccountWithNFCCancelledMessage, AppResources.OK, null);
                        break;
                    }
                case MessagePageType.UpgradeAccountWithNFCAborted:
                    {
                        PageId = "AP227";
                        SetData(AppResources.UpgradeAccountWithNFCAbortedHeader, AppResources.UpgradeAccountWithNFCAbortedMessage, AppResources.UpgradeAccountWithNFCAbortedAction, false);
                        break;
                    }
                case MessagePageType.PasswordChangeSuccess:
                    {
                        PageId = "AP029";
                        SetData(AppResources.ChangePasswordConfirmHeader, AppResources.ChangePasswordConfirmMessage, AppResources.ChangePasswordConfirmAction, true);
                        break;
                    }
                case MessagePageType.PhoneNumberChangeSuccess:
                    {
                        PageId = "AP030";
                        SetData(AppResources.ChangePhoneNumberConfirmHeader, AppResources.ChangePhoneNumberConfirmMessage, AppResources.ChangePasswordConfirmAction, true);
                        break;
                    }
                case MessagePageType.NotActivated:
                    {
                        PageId = "AP021";
                        SetData(AppResources.NotActivatedHeader, AppResources.NotActivatedMessage, AppResources.NotActivatedButton, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.ReactivationNeeded:
                    {
                        PageId = "AP217";
                        SetData(AppResources.ReactivationNeededHeader, AppResources.ReactivationNeededMessage, AppResources.OK, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.ActivationPendingNoSMSCheck:
                    {
                        PageId = "AP259";
                        SetData(AppResources.AccountNotYetActivatedHeader, AppResources.AccountNotYetActivated, AppResources.OK, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.UpgradeAndAuthenticateAborted:
                    {
                        PageId = "AP286";
                        SetData(AppResources.AP286_Header, AppResources.AP286_Message, AppResources.OK, false, PopToRootCommand);
                        break;
                    }
                case MessagePageType.UpgradeAndAuthenticateFailed:
                    {
                        PageId = "AP275 ";
                        SetData(AppResources.AP275_Header, AppResources.AP275_Message, AppResources.LoginButton, null, new AsyncCommand(
                            async () =>
                            {
                                AppSession.Process = Process.NotSet;
                                await NavigationService.ShowMessagePage(MessagePageType.LoginSuccess, data);
                            }));
                        break;
                    }
            }
        }

        protected ICommand PopToRootCommand
        {
            get
            {
                return new Command(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        NavigationService.PopToRoot(true);
                    });
                });
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IA11YService>().Speak(FooterText);
        }

        public override bool OnBackButtonPressed()
        {
            if (ButtonCommand?.CanExecute(null) == true)
            {
                ButtonCommand.Execute(null);
            }

            return true;
        }
    }
}


