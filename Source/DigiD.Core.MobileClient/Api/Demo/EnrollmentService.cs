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
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.EID.Helpers;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.SessionModels;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class EnrollmentService : IEnrollmentServices
    {
        private bool _removeOldApp;
        private int _letterAttempts;
        private int _pollingCount;

        /// <summary>
        /// /apps/challenge
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<EnrollmentChallengeResponse> EnrollmentChallenge(EnrollmentChallengeRequest requestData)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/challenge", requestData, new EnrollmentChallengeResponse
            {
                ApiResult = ApiResult.Ok,
                Challenge = CryptoHelper.GenerateRandom(16).ToBase16()
            });
        }

        /// <summary>
        /// /apps/challenge_response
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<CompleteChallengeResponse> CompleteChallenge(CompleteChallengeRequest requestData)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/challenge_response", requestData, new CompleteChallengeResponse
            {
                ApiResult = ApiResult.Ok,
                IV = CryptoHelper.GenerateRandom(16).ToBase16(),
                SymmetricKey = CryptoHelper.GenerateRandom(32).ToBase16()
            });
        }

        /// <summary>
        /// /apps/pincode
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<CompleteActivationResponse> CompleteActivationAsync(PincodeRequest requestData)
        {
            await Task.Delay(0);

            var apiResult = ApiResult.Ok;

            if (AppSession.Process == Process.AppActivateWithRDA && !_removeOldApp)
                apiResult = ApiResult.Pending;
            else if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                apiResult = ApiResult.Pending;

            return DemoHelper.Log("/apps/pincode", requestData, new CompleteActivationResponse
            {
                ApiResult = apiResult,
                AuthenticationLevel = AppSession.Process == Process.AppActivationViaRequestStation ? 25 : 20,
            });
        }

        /// <summary>
        /// /apps/auth
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BasicAuthenticationResponse> BasicAuthenticate(BasicAuthenticateRequest requestData)
        {
            await Task.Delay(0);

            _removeOldApp = requestData.RemoveOldApp;

            var response = new BasicAuthenticationResponse
            {
                SessionId = DemoHelper.NewSession()
            };

            var user = Common.Mobile.Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.UserName == requestData.Username && x.Password == requestData.Password);

            if (user != null)
            {
                DependencyService.Get<IDemoSettings>().UserId = user.UserId;

                if (DebugConstants.IsClassifiedDeceased)
                {
                    response.ApiResult = ApiResult.Nok;
                    response.ErrorMessage = "classified_deceased";
                    return response;
                }

                response.ApiResult = ApiResult.Ok;
                response.activation_method = user.ActivationMethod;
                response.IsSmsCheckRequested = user.IsSmsCheckRequested;
            }
            else
            {
                response.ApiResult = ApiResult.Nok;
                response.ErrorMessage = "invalid";
            }

            return DemoHelper.Log("/apps/auth", requestData, response);
        }

        /// <summary>
        /// /apps/session
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<SessionResponse> InitSession(BasicAuthenticationSessionRequest requestData)
        {
            await Task.Delay(0);

            var result = new SessionResponse();

            if (requestData.SMSCode == Common.Mobile.Constants.DemoConstants.SMS_CODE || requestData.SMSCode == null)
            {
                result.ApiResult = ApiResult.Ok;
                result.AppId = Guid.NewGuid().ToString();
            }
            else
            {
                result.ApiResult = ApiResult.Nok;
                result.ErrorMessage = "smscode_incorrect";
            }

            return DemoHelper.Log("/apps/session", requestData, result);
        }

        /// <summary>
        /// /apps/resend_sms
        /// </summary>
        /// <param name="spoken"></param>
        /// <returns></returns>
        public async Task<BaseResponse> ResendSMS(bool spoken)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/resend_sms", new ResendSmsRequest(spoken), new BaseResponse
            {
                ApiResult = ApiResult.Ok
            });
        }

        /// <summary>
        /// /apps/sms
        /// </summary>
        /// <returns></returns>
        public async Task<RequestSmsResponse> SendSMS()
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/sms", new BaseRequest(), new RequestSmsResponse
            {
                ApiResult = ApiResult.Ok,
                PhoneNumber = "+31612345678",
            });
        }

        /// <summary>
        /// /apps/activationcode
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<ActivationLetterResponse> CompleteLetterActivation(LetterActivationRequest requestData)
        {
            await Task.Delay(0);

            _letterAttempts++;
            ActivationLetterResponse result;
            if (requestData.ActivationCode == Common.Mobile.Constants.DemoConstants.ACTIVATION_CODE)
            {
                _letterAttempts = 0;
                result = new ActivationLetterResponse
                {
                    ApiResult = ApiResult.Ok
                };
            }
            else if (_letterAttempts == 3)
                result = new ActivationLetterResponse
                {
                    ApiResult = ApiResult.Blocked
                };
            else
                result = new ActivationLetterResponse
                {
                    ErrorMessage = "invalid",
                    ApiResult = ApiResult.Nok,
                    DateLetterSent = DateTime.Now.AddDays(-3),
                    RemainingAttempts = 3 - _letterAttempts,
                };

            return DemoHelper.Log("/apps/activationcode", requestData, result);
        }

        /// <summary>
        /// /apps/letter
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> InitSessionLetterActivation()
        {
            await Task.Delay(0);
            _pollingCount = 0;
            return DemoHelper.Log("/apps/letter", new BaseRequest(), new BaseResponse { ApiResult = ApiResult.Ok });
        }

        /// <summary>
        /// /apps/letter_poll
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> LetterActivationPolling()
        {
            await Task.Delay(0);

            _pollingCount++;
            return DemoHelper.Log("/apps/letter_poll", new BaseRequest(), new BaseResponse { ApiResult = _pollingCount == 3 ? ApiResult.Ok : ApiResult.Pending });
        }

        /// <summary>
        /// /apps/rda_activation
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> InitSessionActivationRDA()
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/rda_activation", new BaseRequest(), new InitSessionResponse
            {
                ApiResult = ApiResult.Ok,
                SessionId = DemoHelper.NewSession(AuthenticationActions.RDAUpgrade)
            });
        }

        /// <summary>
        /// /apps/rda_activation_verified
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> VerifyRDAActivation()
        {
            await Task.Delay(0);
            return DemoHelper.Log("/apps/rda_activation_verified", new BaseRequest(), new BaseResponse { ApiResult = ApiResult.Ok });
        }

        /// <summary>
        /// /apps/activationcode_session
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<InitSessionResponse> InitSessionActivationCode(ActivationCodeSessionRequest requestData)
        {
            await Task.Delay(0);
            _letterAttempts = 0;

            return DemoHelper.Log("/apps/activationcode_session", requestData, new InitSessionResponse
            {
                ApiResult = ApiResult.Ok,
                SessionId = DemoHelper.NewSession(requestData.RequestNewLetter ? AuthenticationActions.ReRequestLetter : AuthenticationActions.ActivationByLetter)
            });
        }

        /// <summary>
        /// /apps/activate/start
        /// </summary>
        /// <returns></returns>
        public async Task<StartSessionResponse> InitSessionActivationByApp()
        {
            await Task.Delay(0);
            _pollingCount = 0;

            return DemoHelper.Log("/apps/activate/start", new BaseRequest(), new StartSessionResponse
            {
                ApiResult = ApiResult.Ok,
                AppSessionId = DemoHelper.NewSession(),
                At = DateHelper.GetEpochSeconds()
            });
        }

        /// <summary>
        /// /apps/authentication_status
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticationStatusResponse> ActivationByAppPolling()
        {
            await Task.Delay(1000);
            _pollingCount++;
            var status = ApiResult.Pending;
            var sessionReceived = false;

            switch (_pollingCount)
            {
                case 1:
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var accounts = Common.Mobile.Constants.DemoConstants.DemoUsers
                            .Where(x => !string.IsNullOrEmpty(x.Password))
                            .Select(x => $"{x.UserName}-{x.Password}")
                            .ToArray();

                        var account = await Application.Current.MainPage.DisplayActionSheet("Demo account", "Kies een demo account", null, accounts);
                        var userName = account.Split('-')[0];
                        var password = account.Split('-')[1];

                        var user = Common.Mobile.Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.UserName == userName && x.Password == password);

                        if (user != null)
                            DependencyService.Get<IDemoSettings>().UserId = user.UserId;
                    });
                    break;
                case 2:
                    sessionReceived = true;
                    break;
                case 4:
                    status = ApiResult.PendingConfirmed;
                    break;
                case 6:
                    status = ApiResult.Ok;
                    break;
            }

            return DemoHelper.Log("/apps/authentication_status", new BaseRequest(), new AuthenticationStatusResponse
            {
                ApiResult = status,
                SessionReceived = sessionReceived
            });
        }

        /// <summary>
        /// /apps/activationcode_account
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<ActivationLetterResponse> ActivateAccountByApp(LetterActivationRequest requestData)
        {
            await Task.Delay(0);
            _letterAttempts++;

            if (requestData.ActivationCode == Common.Mobile.Constants.DemoConstants.ACTIVATION_CODE)
            {
                _letterAttempts = 0;
                return new ActivationLetterResponse
                {
                    ApiResult = ApiResult.Ok
                };
            }

            if (_letterAttempts == 3)
                return new ActivationLetterResponse
                {
                    ApiResult = ApiResult.Blocked
                };

            return DemoHelper.Log("/apps/activationcode_account", requestData, new ActivationLetterResponse
            {
                ErrorMessage = "activation_code_not_correct",
                ApiResult = ApiResult.Nok,
                RemainingAttempts = 3 - _letterAttempts,
                DateLetterSent = DateTime.Now.AddDays(-3),
            });
        }

        /// <summary>
        /// /apps/account_requests/start
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<RequestAccountResponse> InitSessionAccountRequest(RequestAccountRequest requestData)
        {
            await Task.Delay(0);

            var user = Common.Mobile.Constants.DemoConstants.DemoUsers.First(x => x.Bsn == requestData.Bsn);

            RequestAccountResponse result;
            if (requestData.HouseNumber == user.HouseNumber && requestData.Postalcode == user.Postalcode)
            {
                DependencyService.Get<IDemoSettings>().UserId = user.UserId;

                result =  new RequestAccountResponse
                {
                    ApiResult = ApiResult.Ok,
                    AppSessionId = DemoHelper.NewSession()
                };
            }
            else
            {
                result = new RequestAccountResponse
                {
                    ApiResult = ApiResult.Nok,
                    ErrorMessage = "invalid_parameters"
                };
            }

            return DemoHelper.Log("/apps/account_requests/start", requestData, result);
        }

        /// <summary>
        /// /apps/account_requests/existing_application
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AccountRequestsCheckApp(BaseRequest requestData)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/account_requests/existing_application", requestData, new BaseResponse { ApiResult = ApiResult.Ok });
        }

        /// <summary>
        /// /apps/account_requests/replace_application
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AccountRequestsReplaceApp(ReplaceExistingApplicationRequest requestData)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/account_requests/replace_application", requestData, new BaseResponse { ApiResult = ApiResult.Ok });
        }

        /// <summary>
        /// /apps/account_requests/brp_poll
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AccountRequestsBrpPoll(BaseRequest requestData)
        {
            await Task.Delay(100);

            BaseResponse result;
            if (DebugConstants.IsClassifiedDeceased)
                result = new BaseResponse
                {
                    ApiResult = ApiResult.Nok,
                    ErrorMessage = "gba_deceased"
                };
            else
                result =new BaseResponse { ApiResult = ApiResult.Ok };

            return DemoHelper.Log("/apps/account_requests/brp_poll", requestData, result);
        }

        /// <summary>
        /// /apps/account_requests/existing_account
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AccountRequestsCheckAccount(BaseRequest requestData)
        {
            await Task.Delay(1);
            return DemoHelper.Log("/apps/account_requests/existing_account", requestData, new BaseResponse { ApiResult = ApiResult.Ok });
        }

        /// <summary>
        /// /apps/account_requests/replace_account
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AccountRequestsReplaceAccount(ReplaceExistingAccountRequest requestData)
        {
            await Task.Delay(0);
            return DemoHelper.Log("/apps/account_requests/replace_account", requestData, new BaseResponse { ApiResult = ApiResult.Ok });
        }

        public async Task<BaseResponse> SkipRda()
        {
            await Task.Delay(0);
            return DemoHelper.Log("/apps/skip_rda", new BaseRequest(), new BaseResponse { ApiResult = ApiResult.Ok });
        }
    }
}
