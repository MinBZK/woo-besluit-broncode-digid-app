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
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class AP079ViewModel : BaseViewModel
    {
        public string Bsn { get; set; }
        public string DateOfBirth { get; set; }
        public string Postalcode { get; set; }
        public string HouseNumber { get; set; }
        public string HouseNumberSuffix { get; set; }

        public bool IsValidBrpCheck { get; set; } = true;
        public bool IsValidBsn { get; set; } = true;
        public bool IsValidDob { get; set; } = true;
        public bool IsValidPostalcode { get; set; } = true;
        public bool IsValidHouseNumber { get; set; } = true;

        public bool IsNextButtonEnabled => (IsValid && CanExecute) || DependencyService.Get<IA11YService>().IsInVoiceOverMode();

        public string BrpErrorText { get; private set; }

        private bool _isValid;
        public bool IsValid
        {
            get
            {
                Validate();
                return _isValid;
            }
        }

        public AP079ViewModel()
        {
            PageId = "AP079";

            ButtonCommand = new AsyncCommand(async () =>
            {
                if (IsValid)
                    await RequestAccount();
            }, () => CanExecute && IsValid || DependencyService.Get<IA11YService>().IsInVoiceOverMode());

            NavCloseCommand = new AsyncCommand(async () => await NavigationService.PopToRoot(true));
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            CanExecute = true;
        }

        public Command<string> ValidateCommand => new Command<string>(Validate);

        private async Task RequestAccount()
        {
            if (!CanExecute)
                return;

            CanExecute = false;
            IsValidBrpCheck = true;

            if (!await DependencyService.Get<IGeneralServices>().SslPinningCheck())
            {
                await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                return;
            }

            var language = DependencyService.Get<IGeneralPreferences>().Language.ToUpper(CultureInfo.InvariantCulture);

            if (language == "EN")
            {
                var confirm = await DependencyService.Get<IAlertService>().DisplayAlert(MobileAppResources.M20_Title, MobileAppResources.M20_Message, MobileAppResources.M20_English, MobileAppResources.M20_Dutch);

                if (!confirm)
                    language = "NL";
            }

            DialogService.ShowProgressDialog();
            HttpSession.ActivationSessionData = new ActivationSessionData { ActivationMethod = ActivationMethod.RequestNewDigidAccount };

            var isDemoUser = Common.Mobile.Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.Bsn == Bsn) != null;
            App.RegisterServices(isDemoUser ? AppMode.Demo : AppMode.Normal);

            var svc = DependencyService.Get<IEnrollmentServices>();

            // gegevens verzamelen en dan
            var ds = CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.DateSeparator;
            var dob = DateOfBirth.StartsWith($"00{ds}00{ds}") ? $"{DateOfBirth.Split(ds)[2]}0000" : DateOfBirth.ToDate().ToString("yyyyMMdd");

            var request = new RequestAccountRequest
            {
                Bsn = Bsn,
                DateOfBirth = dob,
                Postalcode = Postalcode.ToUpper(CultureInfo.InvariantCulture),
                HouseNumber = HouseNumber,
                HouseNumberSuffix = HouseNumberSuffix,
                Language = language,
                HasNfcSupport = App.HasNfc
            };

            var raResponse = await svc.InitSessionAccountRequest(request);

            if (raResponse.ApiResult == ApiResult.Ok)
            {
                HttpSession.AppSessionId = raResponse.AppSessionId;

                //Poll kern if BRP-controle nog loopt
                var pollRequest = new BaseRequest();
                var pollResult = await svc.AccountRequestsBrpPoll(pollRequest);

                while (pollResult.ApiResult == ApiResult.Pending)
                {
                    await Task.Delay(1000);
                    pollResult = await svc.AccountRequestsBrpPoll(pollRequest);
                }

                if (pollResult.ApiResult == ApiResult.Ok)
                {
                    // controleren of er al een lopende aanvraag is met ingevoerde BSN
                    await CheckExistingRequest();
                }
                else
                    await HandleInvalidResponse(pollResult);
            }
            else
                await HandleInvalidInput(raResponse);

            DialogService.HideProgressDialog();

            CanExecute = true;
        }

        private async Task HandleInvalidResponse(BaseResponse pollResult)
        {
            if (string.IsNullOrEmpty(pollResult.ErrorMessage))
            {
                await NavigationService.ShowMessagePage(MessagePageType.RequestAccountNotAvailable);
                return;
            }

            switch (pollResult.ErrorMessage)
            {
                case "gba_timeout":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountBRPTimeout);
                    break;
                case "mismatch_personal_data_with_gba":
                    IsValidBrpCheck = false;
                    // Omdat de foutmelding een LiveRegion is, eerst tekst leegmaken en vervolgens weer vullen
                    // LiveRegions worden voorgelezen als de tekst wijzigt
                    BrpErrorText = "";
                    BrpErrorText = AppResources.AP079_ErrorBRP;
                    break;
                case "gba_emigrated_RNI":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountAddressAbroad);
                    break;
                case "gba_under_investigation":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountNotLatestAddress);
                    break;
                case "gba_address_under_investigation":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountAddressUnderInvestigating);
                    break;
                case "gba_deceased":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountDeceased);
                    break;
                case "BSN_unsubscribed":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountAccountBlocked);
                    break;
                case "application_too_soon":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountTooOftenDay);
                    break;
                case "too_many_applications_this_month":
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountTooOftenMonth, (int)pollResult.Payload.max_amount_of_account_application_per_month);
                    break;
                default:
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountNotAvailable);
                    break;
            }
        }


        private async Task HandleInvalidInput(RequestAccountResponse raResponse)
        {
            if (raResponse.ErrorMessage != "invalid_parameters")
            {
                if (raResponse.Delay > 0)
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountBlockedTemporarily, raResponse.Delay);
                else
                    await NavigationService.ShowMessagePage(MessagePageType.RequestAccountNotAvailable);
            }
            else
            {
                IsValidBrpCheck = false;
                // Omdat de foutmelding een LiveRegion is, eerst tekst leegmaken en vervolgens weer vullen
                // LiveRegions worden voorgelezen als de tekst wijzigt
                BrpErrorText = "";
                BrpErrorText = AppResources.AP079_ErrorBRP;
            }
        }

        private async Task CheckExistingRequest()
        {
            var svc = DependencyService.Get<IEnrollmentServices>();

            var baseResponse = await svc.AccountRequestsCheckApp(new BaseRequest());

            if (baseResponse.ApiResult == ApiResult.Ok) // er is geen lopende aanvraag
            {
                // kijken of er al een account voor ingevoerde BSN is.
                await CheckExistingAccount();
                return;
            }

            if (baseResponse.ApiResult == ApiResult.Pending)
            {
                await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RequestAccountExistingApp), false, async tryAgain =>
                {
                    baseResponse = await svc.AccountRequestsReplaceApp(new ReplaceExistingApplicationRequest(tryAgain));

                    if (baseResponse.ApiResult == ApiResult.Ok && tryAgain)
                    {
                        // Gebruiker geeft aan nieuwe aanvraag te willen doen.
                        await StartSession();
                        return;
                    }

                    await NavigationService.PopToRoot();

                }));
            }
            else
                await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
        }

        private async Task CheckExistingAccount()
        {
            var svc = DependencyService.Get<IEnrollmentServices>();
            var baseResponse = await svc.AccountRequestsCheckAccount(new BaseRequest());

            if (baseResponse.ApiResult == ApiResult.Ok)
            {
                // nog geen account met ingevoerde BSN of opnieuw aanvragen.
                // start de sessie
                await StartSession();
            }
            else if (baseResponse.ApiResult == ApiResult.Pending)
            {
                await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RequestAccountExistingAccount), false, async (newRequest) =>
                {
                    baseResponse = await svc.AccountRequestsReplaceAccount(new ReplaceExistingAccountRequest(newRequest));

                    if (baseResponse.ApiResult == ApiResult.Ok && newRequest)
                    {
                        // gebruiker wil account opnieuw aanvragen.
                        // start de sessie
                        await StartSession();
                        return;
                    }

                    await NavigationService.PopToRoot();
                }));
            }
            else
            {
                await NavigationService.ShowMessagePage(MessagePageType.RequestAccountAccountBlocked);
            }
        }

        private async Task StartSession()
        {

            DialogService.ShowProgressDialog();
            var result = await ActivationHelper.InitSession();

            if (result.ApiResult == ApiResult.Ok)
            {
                var activationResult = await ActivationHelper.StartChallenge(result.AppId);

                if (activationResult.ApiResult == ApiResult.Ok)
                    await DependencyService.Get<INavigationService>().PushAsync(new PinCodeViewModel(new PinCodeModel(PinEntryType.Enrollment)
                    {
                        IV = activationResult.IV
                    }));
                else
                    await DependencyService.Get<INavigationService>().ShowMessagePage(activationResult.MessagePageType);
            }

            DialogService.HideProgressDialog();
        }

        public void Validate(string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(Bsn):
                    IsValidBsn = Bsn.IsValidBsn();
                    break;
                case nameof(DateOfBirth):
                    IsValidDob = IsValidDateOfBirth(DateOfBirth);
                    break;
                case nameof(Postalcode):
                    IsValidPostalcode = Postalcode != null && Postalcode.IsValidZipcode();
                    break;
                case nameof(HouseNumber):
                    IsValidHouseNumber = int.TryParse(HouseNumber, out int hn) && hn > 0;
                    break;
                case "All":
                    Validate(nameof(Bsn));
                    OnPropertyChanged(nameof(Bsn));
                    if (!IsValidBsn)
                        return;

                    Validate(nameof(DateOfBirth));
                    OnPropertyChanged(nameof(DateOfBirth));
                    if (!IsValidDob)
                        return;

                    Validate(nameof(Postalcode));
                    OnPropertyChanged(nameof(IsValidPostalcode));
                    if (!IsValidPostalcode)
                        return;

                    Validate(nameof(HouseNumber));
                    OnPropertyChanged(nameof(IsValidHouseNumber));
                    break;
            }
            var input = $"{Bsn}{DateOfBirth}{Postalcode}{HouseNumber}";
            // lengte minimaal 9 + 10 + 6 + 1
            _isValid = input.Length >= 26 && IsValidBsn && IsValidDob && IsValidPostalcode && IsValidHouseNumber;
        }

        private static bool IsValidDateOfBirth(string dateOfBirth)
        {
            var ds = CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.DateSeparator;
            return !string.IsNullOrEmpty(dateOfBirth) && (dateOfBirth.StartsWith($"00{ds}00{ds}") ||
                    DateTime.TryParse(dateOfBirth, CultureInfo.DefaultThreadCurrentCulture, DateTimeStyles.None, out var date) && DateTime.Now > date);
        }
    }
}
