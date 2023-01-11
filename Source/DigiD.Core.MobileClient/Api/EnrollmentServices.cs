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
﻿using System.Threading.Tasks;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;

namespace DigiD.Api
{
    internal class EnrollmentServices : IEnrollmentServices
    {
        /// <summary>
        /// Start een nieuwe app activatie sessie door te authenticeren met gebruikersnaam, wachtwoord en/of sms verificatie
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>`
        public async Task<BasicAuthenticationResponse> BasicAuthenticate(BasicAuthenticateRequest requestData)
        {
            return await HttpHelper.PostAsync<BasicAuthenticationResponse>("/apps/auth", requestData);
        }

        /// <summary>
        /// Geeft de app sessie (het antwoord past zich aan afhankelijk van het soort app sessie)
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<SessionResponse> InitSession(BasicAuthenticationSessionRequest requestData)
        {
            return await HttpHelper.PostAsync<SessionResponse>("/apps/session", requestData);
        }

        public async Task<EnrollmentChallengeResponse> EnrollmentChallenge(EnrollmentChallengeRequest requestData)
        {
            return await HttpHelper.PostAsync<EnrollmentChallengeResponse>("/apps/challenge_activation", requestData);
        }

        public async Task<CompleteChallengeResponse> CompleteChallenge(CompleteChallengeRequest requestData)
        {
            return await HttpHelper.PostAsync<CompleteChallengeResponse>("/apps/challenge_response", requestData);
        }

        /// <summary>
        /// Activates the device.
        /// </summary>
        /// <returns>The device.</returns>
        public async Task<CompleteActivationResponse> CompleteActivationAsync(PincodeRequest requestData)
        {
            return await HttpHelper.PostAsync<CompleteActivationResponse>("/apps/pincode", requestData);
        }

        /// <summary>
        /// Het proces waarin de activatiebrief voor de DigiD app aangevraagd wordt
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> InitSessionLetterActivation()
        {
            var response = await HttpHelper.PostAsync<BaseResponse>("/apps/letter", new BaseRequest());
            return response;
        }

        public async Task<BaseResponse> InitSessionActivationRDA()
        {
            var data = new
            {
                app_session_id = HttpSession.AppSessionId,
                re_request = false
            };

            return await HttpHelper.PostAsync<BaseResponse>("/apps/rda_activation", data);
        }

        /// <summary>
        /// Pollen of de activatiebrief verzonden is na gba-check
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> LetterActivationPolling()
        {
            var response = await HttpHelper.PostAsync<BaseResponse>("/apps/letter_poll", new BaseRequest());
            return response;
        }

        /// <summary>
        /// Geeft de app sessie (het antwoord past zich aan afhankelijk van het soort app sessie)
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<InitSessionResponse> InitSessionActivationCode(ActivationCodeSessionRequest requestData)
        {
            return await HttpHelper.PostAsync<InitSessionResponse>("/apps/activationcode_session", requestData);
        }

        /// <summary>
        /// Activeer de app met de activeringscode na het ontvangen van de activatiebrief
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<ActivationLetterResponse> CompleteLetterActivation(LetterActivationRequest requestData)
        {
            return await HttpHelper.PostAsync<ActivationLetterResponse>("/apps/activationcode", requestData);
        }

        /// <summary>
        /// Verstuurt een nieuwe sms als de vorige niet aan is gekomen
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> ResendSMS(bool spoken)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/resend_sms", new ResendSmsRequest(spoken));
        }

        public async Task<RequestSmsResponse> SendSMS()
        {
            return await HttpHelper.PostAsync<RequestSmsResponse>("/apps/sms", new BaseRequest());
        }

        public async Task<BaseResponse> VerifyRDAActivation()
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/rda_activation_verified", new BaseRequest());
        }

        public async Task<StartSessionResponse> InitSessionActivationByApp()
        {
            return await HttpHelper.GetAsync<StartSessionResponse>("/apps/activate/start");
        }

        public async Task<AuthenticationStatusResponse> ActivationByAppPolling()
        {
            return await HttpHelper.PostAsync<AuthenticationStatusResponse>("/apps/authentication_status", new BaseRequest());
        }

        public async Task<ActivationLetterResponse> ActivateAccountByApp(LetterActivationRequest requestData)
        {
            return await HttpHelper.PostAsync<ActivationLetterResponse>("/apps/activationcode_account", requestData);
        }

        public async Task<RequestAccountResponse> InitSessionAccountRequest(RequestAccountRequest requestData)
        {
            return await HttpHelper.PostAsync<RequestAccountResponse>("/apps/account_requests/start", requestData);
        }

        public async Task<BaseResponse> AccountRequestsCheckApp(BaseRequest requestData)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/existing_application", requestData);
        }

        public async Task<BaseResponse> AccountRequestsReplaceApp(ReplaceExistingApplicationRequest requestData)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/replace_application", requestData);
        }

        public async Task<BaseResponse> AccountRequestsCheckAccount(BaseRequest requestData)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/existing_account", requestData);
        }

        public async Task<BaseResponse> AccountRequestsReplaceAccount(ReplaceExistingAccountRequest requestData)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/replace_account", requestData);
        }

        public async Task<BaseResponse> AccountRequestsBrpPoll(BaseRequest requestData)
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/brp_poll", requestData);
        }

        public async Task<BaseResponse> SkipRda()
        {
            return await HttpHelper.PostAsync<BaseResponse>("/apps/skip_rda", new BaseRequest());
        }
    }
}
