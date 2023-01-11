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
using DigiD.Common.Constants;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class AuthenticationService : IAuthenticationService
    {
        private int _attempts;

        public async Task<BaseResponse> SessionStatus()
        {
            await Task.Delay(20);

            var session = Common.Http.Helpers.DemoHelper.GetSession(AppSession.AuthenticationSessionId);
            BaseResponse result;
            if (session is { IsAuthenticated: true } && session.DateTime != null && DateTime.Now - session.DateTime.Value <= AppConfigConstants.SessionTimeout)
            {
                DemoHelper.ExtendSession(AppSession.AuthenticationSessionId);
                result = new BaseResponse
                {
                    ApiResult = ApiResult.Ok
                };
            }
            else
            {
                result = new BaseResponse
                {
                    ApiResult = ApiResult.Nok,
                    ErrorMessage = "no_session"
                };
            }

            return DemoHelper.Log("/apps/session_status", new BaseRequest(), result);
        }

        public async Task<StartSessionBaseResponse> InitSessionAuthentication()
        {
            await Task.Delay(0);

            var result = new StartSessionBaseResponse
            {
                ApiResult = ApiResult.Ok,
                AppSessionId = DemoHelper.NewSession(AuthenticationActions.RequestSession),
                At = DateHelper.GetEpochSeconds()
            };

            return DemoHelper.Log("/apps/request_session", new BaseRequest(), result);
        }

        public async Task<ChallengeResponse> Challenge(ChallengeRequest requestData)
        {
            await Task.Delay(0);
            _attempts = 0;

            var session = Common.Http.Helpers.DemoHelper.GetSession(requestData.AuthenticationSessionId);

            return DemoHelper.Log("/apps/challenge", requestData, new ChallengeResponse
            {
                ApiResult = ApiResult.Ok,
                IV = session.IV,
                Challenge = session.Challenge
            });
        }

        public async Task<ValidatePinResponse> ValidatePin(ValidatePinRequest requestData)
        {
            await Task.Delay(0);
            _attempts++;

            var session = Common.Http.Helpers.DemoHelper.GetSession(requestData.AuthenticationSessionId);
            var key = Common.NFC.Helpers.StringHelper.HexToByteArray(DependencyService.Get<IMobileSettings>().SymmetricKey);
            var maskedPin = EncryptionHelper.Decrypt(requestData.PIN, Common.NFC.Helpers.StringHelper.HexToByteArray(session.IV), key);

            if (maskedPin == DependencyService.Get<IDemoSettings>().DemoPin)
            {
                _attempts = 0;
                Common.Http.Helpers.DemoHelper.Sessions[requestData.AuthenticationSessionId].IsAuthenticated = true;

                if (DebugConstants.IsClassifiedDeceased)
                {
                    switch (session.Action)
                    {
                        case AuthenticationActions.ChangeAppPIN:
                        case AuthenticationActions.ActivationByLetter:
                        case AuthenticationActions.RDAUpgrade:
                            return new ValidatePinResponse
                            {
                                ApiResult = ApiResult.Nok,
                                ErrorMessage = "classified_deceased"
                            };
                    }
                }

                return DemoHelper.Log("/apps/authenticate", requestData, new ValidatePinResponse
                {
                    ApiResult = ApiResult.Ok,
                    AuthenticationLevel = DependencyService.Get<IMobileSettings>().LoginLevel.ToInt()
                });
            }

            var remainingAttempts = 3 - _attempts;

            return DemoHelper.Log("/apps/authenticate", requestData, new ValidatePinResponse
            {
                ApiResult = remainingAttempts > 0 ? ApiResult.Nok : ApiResult.Blocked,
                RemainingAttempts = remainingAttempts
            });
        }

        public async Task<ConfirmResponse> Confirm(ConfirmRequest requestData)
        {
            await Task.Delay(10);

            if (Common.Http.Helpers.DemoHelper.Sessions.ContainsKey(HttpSession.AppSessionId))
                Common.Http.Helpers.DemoHelper.Sessions.Remove(HttpSession.AppSessionId);

            return DemoHelper.Log("/apps/confirm", requestData, new ConfirmResponse
            {
                ApiResult = ApiResult.Ok
            });
        }

        public async Task<WebSessionInfoResponse> SessionInfo()
        {
            await Task.Delay(10);

            var session = Common.Http.Helpers.DemoHelper.GetSession(HttpSession.AppSessionId);

            var result = new WebSessionInfoResponse
            {
                ApiResult = ApiResult.Ok,
                AuthenticationLevel = session.LoginLevel
            };

            if (DemoHelper.CurrentUser.EIDASUser)
            {
                if (DemoHelper.CurrentUser.EIDASSuccess)
                    result.HashedPIP = "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS";
                else
                {
                    result.ApiResult = ApiResult.Nok;
                    result.ErrorMessage = "aborted_contact_helpdesk";
                    return result;
                }
            }

            switch (session.Action)
            {
                case "webservice":
                case "app2app":
                    result.WebService = "DigiD Demo";
                    break;
                default:
                    result.Action = session.Action;
                    break;
            }
            return DemoHelper.Log("/apps/web_session_information", new BaseRequest(), result);
        }

        public async Task<ApiResult> AbortAuthentication(string abortCode)
        {
            await Task.Delay(0);
            return DemoHelper.Log("/apps/abort_authentication", new { abortCode }, ApiResult.Ok);
        }
    }
}
