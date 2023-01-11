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
﻿using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class ActivationRequestStationViewModel : BaseActivationViewModel
    {
        public string VerificationCode { get; set; }
        public string VerificationCode4A11Y => string.Join(",", VerificationCode.ToCharArray());

        private readonly RequestStationSessionResponse _response;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _removeOldApp;

#if A11YTEST
        public ActivationRequestStationViewModel() : this( new RequestStationSessionResponse {
            ActivationCode = "A11Y",
            AppSessionId = "a11yAppSessionId",
            LoadBalanceCookie = "a11yLoadBalanceCookie"           
        }) { }
#endif

        public ActivationRequestStationViewModel(RequestStationSessionResponse response)
        {
            PageId = "AP060";
            VerificationCode = response.ActivationCode;
            HttpSession.AppSessionId = response.AppSessionId;

            _response = response;

            NavCloseCommand = CancelCommand;

            Task.Run(async () => await StartPolling());
        }

        private async Task StartPolling()
        {
            var request = new CompleteActivationPollRequest
            {
                ActivationCode = _response.ActivationCode,
                RemoveOldApp = _removeOldApp
            };

            var result = await DependencyService.Get<IRequestStationServices>().CompleteActivationPoll(request, _cts.Token);

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    var activationResult = await ActivationHelper.StartChallenge(result.AppId);

                    if (activationResult.ApiResult == ApiResult.Ok)
                    {
                        NextCommand = new AsyncCommand(async () =>
                            {
                                IsNextCommandExecuted = true;
                                await NavigationService.PushAsync(new PinCodeViewModel(new PinCodeModel(PinEntryType.Enrollment)
                                {
                                    IV = activationResult.IV
                                }));
                            },
                            () => !IsNextCommandExecuted);

                        SetNavbar(false);
                        IsBlurVisible = true;

                    }
                    else
                        await NavigationService.ShowMessagePage(activationResult.MessagePageType);
                    break;
                case ApiResult.Nok:
                    {
                        switch (result.ErrorMessage.ToUpper())
                        {
                            case "TOO_MANY_APPS":
                                _removeOldApp = true;
                                await NavigationService.PushModalAsync(new ToManyActiveAppsViewModel(result, async () => await StartPolling()));
                                break;
                            case "APP_ACTIVATION_CODE_NOT_VALID":
                                await NavigationService.ShowMessagePage(MessagePageType.ActivateViaRequestStationCodeExpired);
                                break;
                            default:
                                await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                                break;
                        }
                        break;
                    }
                case ApiResult.SessionNotFound:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                        break;
                    }
                case ApiResult.Disabled:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                        break;
                    }
                case ApiResult.Aborted:
                    {
                        await App.CancelSession(true, async () => await NavigationService.PopToRoot(true));
                        break;
                    }
                default:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
                        break;
                    }
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new Command(() =>
               {
                   CanExecute = false;
                   _cts.Cancel();
                   CanExecute = true;
               }, () => CanExecute);
            }
        }
    }
}
