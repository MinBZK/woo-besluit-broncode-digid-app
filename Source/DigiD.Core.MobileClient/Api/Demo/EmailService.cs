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
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Interfaces;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class EmailService : IEmailService
    {
        private int _attempts;

        /// <summary>
        /// /apps/email/status
        /// </summary>
        /// <returns></returns>
        public async Task<EmailStatusResponse> Status()
        {
            await Task.Delay(0);

            var response = new EmailStatusResponse
            {
                ApiResult = ApiResult.Ok
            };

            var user = DemoHelper.CurrentUser;
            var settings = DependencyService.Get<IDemoSettings>();

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.EmailAddress))
                {
                    if ((settings.EmailAddressVerified.HasValue && settings.EmailAddressVerified.Value) || user.IsEmailAddressVerified)
                    {
                        response.EmailAddress = user.EmailAddress;
                        response.EmailStatus = EmailStatus.Verified;
                    }
                    else
                    {
                        response.NoVerifiedEmailAddress = user.EmailAddress;
                        response.EmailStatus = EmailStatus.NotVerified;
                    }
                }
                else
                {
                    response.EmailStatus = EmailStatus.None;
                    response.UserActionNeeded = true;
                }

                if (settings.EmailCheckDate == null)
                    response.UserActionNeeded = user.UserActionNeeded;
                else
                    response.UserActionNeeded = (DateTime.Now - settings.EmailCheckDate.Value).TotalMinutes > 2;
            }

            return DemoHelper.Log("/apps/email/status", new BaseRequest(), response);
        }

        /// <summary>
        /// /apps/email/register
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public async Task<EmailRegisterResponse> Register(string emailAddress)
        {

            await Task.Delay(0);
            _attempts = 0;

            if (emailAddress == null)
                DependencyService.Get<IDemoSettings>().EmailCheckDate = DateTime.Now;
            else
            {
                DependencyService.Get<IDemoSettings>().EmailAddress = emailAddress;
                DependencyService.Get<IDemoSettings>().EmailAddressVerified = false;
            }

            return DemoHelper.Log("/apps/email/register", new EmailRegisterRequest
            {
                EmailAddress = emailAddress
            }, new EmailRegisterResponse
            {
                ApiResult = ApiResult.Ok,
                EmailAddress = emailAddress
            });
        }

        /// <summary>
        /// /apps/email/confirm
        /// </summary>
        /// <param name="isValid"></param>
        /// <returns></returns>
        public async Task<BaseResponse> Confirm(bool isValid)
        {
            await Task.Delay(0);
            DependencyService.Get<IDemoSettings>().EmailCheckDate = DateTime.Now;

            return DemoHelper.Log("/apps/email/confirm", new EmailConfirmRequest
            {
                IsConfirmed = isValid
            }, new BaseResponse
            {
                ApiResult = ApiResult.Ok
            });
        }

        /// <summary>
        /// /apps/email/verify
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<EmailVerifyResponse> Verify(string code)
        {
            await Task.Delay(0);

            _attempts++;
            EmailVerifyResponse response;
            if (code.ToUpper() == Common.Mobile.Constants.DemoConstants.EMAIL_ACTIVATION_CODE)
            {
                DependencyService.Get<IDemoSettings>().EmailAddressVerified = true;
                DependencyService.Get<IDemoSettings>().EmailCheckDate = DateTime.Now;

                _attempts = 0;
                response = new EmailVerifyResponse
                {
                    ApiResult = ApiResult.Ok,
                };
            }
            else
            {
                var remainingAttempts = 3 - _attempts;

                response = new EmailVerifyResponse
                {
                    ApiResult = ApiResult.Nok,
                    ErrorMessage = remainingAttempts > 0 ? "code_incorrect" : "code_blocked",
                    RemainingAttempts = remainingAttempts
                };
            }

            return DemoHelper.Log("/apps/email/verify", new EmailVerifyRequest
            {
                Code = code
            }, response);
        }
    }
}
