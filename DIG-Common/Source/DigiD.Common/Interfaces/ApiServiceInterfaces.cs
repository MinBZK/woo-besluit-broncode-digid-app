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
﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;

namespace DigiD.Common.Interfaces
{
    public interface IVersionService
    {
        Task<VersionCheckResponse> CheckVersion();
    }

    public interface IApp2AppService
    {
        /// <summary>
        /// Via dit koppelvlak stuurt de DigiD app het SAML AuthnRequest, afkomstig van de afnemer, door naar DigiD Kern.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        Task<InitApp2AppResponse> InitSessionApp2App(App2AppRequest requestData);

        /// <summary>
        /// Via dit koppelvlak vraagt de DigiD app het SAMLart op bij DigiD Kern na een succesvolle SAML authenticatie.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        Task<SamlArtifactResponse> GetSamlArtifact(SamlArtifactRequest requestData);
    }

    public interface IAuthenticationService
    {
        /// <summary>
        /// Als gebruiker de app opent, of als de app geopend wordt via een link, en app stelt vast dat hij een
        /// sessie in zijn geheugen heeft, dan vraagt hij aan Kern of die sessie nog bestaat en geauthentiseerd is
        /// </summary>
        /// <returns></returns>
        Task<BaseResponse> SessionStatus();

        /// <summary>
        /// Als de app geen (geauthentiseerde) sessie heeft, vraagt hij een nieuwe app_sessie aan.
        /// </summary>
        /// <returns></returns>
        Task<StartSessionBaseResponse> InitSessionAuthentication();
        
        Task<ChallengeResponse> Challenge(ChallengeRequest requestData);

        /// <summary>
        /// Via dit koppelvlak vraagt de app DigiD Kern om de pincode te controleren tijdens een authenticatie.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        Task<ValidatePinResponse> ValidatePin(ValidatePinRequest requestData);

        Task<ConfirmResponse> Confirm(ConfirmRequest requestData);

        Task<WebSessionInfoResponse> SessionInfo();

        Task<ApiResult> AbortAuthentication(string abortCode);
    }

    public interface IAccountInformationServices
    {
        Task<TwoFactorResponseModel> GetTwoFactorStatus();
        Task<AccountStatusResponse> GetAccountStatus();
        Task<ApiResult> SetTwoFactor(bool enabled);
    }

    public interface IRequestStationServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Possible errors: * status: "NOK", error: ['invalid', 'account_inactive', 'account_blocked'] (1 van die 3)</returns>
        Task<RequestStationSessionResponse> InitSessionRequestStation(RequestStationSessionRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns>status: ["PENDING", "OK", "NOK"]</returns>
        Task<CompleteActivationPollResponse> CompleteActivationPoll(CompleteActivationPollRequest request, CancellationToken ct);
    }

    public interface IEnrollmentServices
    {
        Task<InitSessionResponse> InitSessionActivationCode(ActivationCodeSessionRequest requestData);
        Task<EnrollmentChallengeResponse> EnrollmentChallenge(EnrollmentChallengeRequest requestData);
        Task<CompleteChallengeResponse> CompleteChallenge(CompleteChallengeRequest requestData);
        Task<CompleteActivationResponse> CompleteActivationAsync(PincodeRequest requestData);
        Task<BasicAuthenticationResponse> BasicAuthenticate(BasicAuthenticateRequest requestData);
        Task<SessionResponse> InitSession(BasicAuthenticationSessionRequest requestData);
        Task<BaseResponse> InitSessionLetterActivation();
        Task<BaseResponse> LetterActivationPolling();
        Task<ActivationLetterResponse> CompleteLetterActivation(LetterActivationRequest requestData);
        Task<BaseResponse> ResendSMS(bool spoken);
        Task<RequestSmsResponse> SendSMS();
        Task<BaseResponse> InitSessionActivationRDA();
        Task<BaseResponse> VerifyRDAActivation();
        Task<StartSessionResponse> InitSessionActivationByApp();
        Task<AuthenticationStatusResponse> ActivationByAppPolling();
        Task<ActivationLetterResponse> ActivateAccountByApp(LetterActivationRequest requestData);
        Task<RequestAccountResponse> InitSessionAccountRequest(RequestAccountRequest requestData);
        Task<BaseResponse> AccountRequestsCheckApp(BaseRequest requestData);
        Task<BaseResponse> AccountRequestsReplaceApp(ReplaceExistingApplicationRequest requestData);
        Task<BaseResponse> AccountRequestsBrpPoll(BaseRequest requestData);
        Task<BaseResponse> AccountRequestsCheckAccount(BaseRequest requestData);
        Task<BaseResponse> AccountRequestsReplaceAccount(ReplaceExistingAccountRequest requestData);
        Task<BaseResponse> SkipRda();
    }

    public interface IRemoteNotificationServices
    {
        Task<BaseResponse> RegisterNotificationToken(string token, bool enabled);
        Task<BaseResponse> UpdateNotificationToken(string token);
    }

    public interface IGeneralServices
    {
        Task<BaseResponse> Cancel(bool cancelledByUser = false, bool timeout = false);
        Task<ConfigResponse> GetConfig();
        Task<bool> SslPinningCheck();
        Task<AppServiceResponse> GetServices();

#if READ_PHOTO
        Task<SessionModels.MrzCodeResponse> GetMrzCodes(string appId);
#endif
    }

    public interface IRdaServices
    {
        Task<InitSessionResponse> InitSessionRDA();
        Task<BaseResponse> Documents();
        Task<RdaSessionResponse> Poll();
        Task<ApiResult> Verified();
        Task<BaseResponse> Cancel();
        Task<RdaStartResponse> Start(string type, EFCardAccess cardAccess);
        Task<SelectResponse> Challenge(string challenge);
        Task<CommandResponse> Authenticate(string authenticate, string challenge);
        Task<CommandResponse> SecureMessaging(List<string> responses);
        Task<RdaSessionResponse> InitForeignDocument(InitForeignDocumentRequestModel model);

        Task<PrepareResponseModel> Prepare(byte[] encryptedNonce);
        Task<MapResponseModel> Map(byte[] mappedNonce);
        Task<AgreeKeyResponseModel> KeyAgreement(byte[] keyAgree);
        Task<MutualAuthResponseModel> MutualAuthenticate(byte[] authenticate, byte[] encryptedNonce = null);
    }

    public interface IEIDServices
    {
        Task<ApiResult> Confirm();
        Task<WidSessionResponse> InitSessionEID(WidSessionRequest requestData);
        Task<ApiResult> CancelAuthenticate();
        Task<ApiResult> AbortAuthentication(AbortAuthenticationRequest requestData);
        Task<BaseResponse> Poll();
    }

    public interface IUsageHistoryService
    {
        Task<List<UsageHistoryModel>> GetUsageHistory(int pageIndex = 0);
    }
}
