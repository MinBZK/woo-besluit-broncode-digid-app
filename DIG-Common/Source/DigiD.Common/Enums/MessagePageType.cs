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
﻿namespace DigiD.Common.Enums
{
    public enum MessagePageType
	{
		AppBlocked,
		ActivationTimeElapsed,
		ActivationCompleted,
		ActivationCancelled,
		ActivationFailed,
        ActivationFailedTooManyDevices,
        ActivationLetterBlocked,
        ActivationLetterExpired,
        ActivationDisabled,
        ActivationPending,
        ActivationFailedTooSoon,
        ActivationFailedTooFast,
        AccountBlocked,
        GBAFailed,
        LoginUnknown,
		LoginSuccess,
	    NoBSN,
        LoginCancelled,
        LoginAborted,
		NoInternetConnection,
		ErrorOccoured,
		InvalidScan,
		ScanTimeout,
        SessionTimeout,
        ChallengeFailed,
        HostUnknown,
        SSLException,
        BrowserUnsupported,
        NoNFCSupport,
        UpgradeAccountWithNFCSuccess,
        UpgradeAccountWithNFCFailed,
        UpgradeAccountWithNFCError,
        UpgradeAccountWithNFCCancelled,
	    UpgradeAccountWithNFCAborted,
        ReRequestLetterSuccess,
        NetworkTimeout,
        PasswordChangeSuccess,
        ActivateSMSAuthenticationSuccess,
        ActivateSMSAuthenticationBlocked,
        PhoneNumberChangeSuccess,
        WIDRandomizeSuccess,
        WIDRandomizeFailed,
        WIDSuspended,
        WIDCancelled,
        WIDFirstUse,
        WIDChangePINSuccess,
        WIDResumePINSuccess,
	    WIDResumePUKSuccess,
        WIDResumeFailed,
        WIDPINBlocked,
        WIDPUKBlocked,
        WIDFailed,
	    WIDLoginFailed,
        VerificationCodeFailed,
        WIDActivationSuccess,
        WIDActivationFailed,
        WIDReActivationSuccess,
        WIDReActivationFailed,
        WIDRetractSuccess,
        WIDNotSupported,
        WIDReaderTimeout,
        WIDActivationNeeded,
        InvalidAuthenticationLevel,
        UnknownError,
        NotActivated,
        ReactivationNeeded,
        InsufficientLoginLevel,
        RDADisabled,
        UpgradeAccountWithNFCNoDocuments,
        UpgradeAccountWithIDCheckerSuccess,
        EmailChange,
        EmailAdd,
        EmailRemove,
        AuthenticationAborted,
        AppActivationWithAppSuccess,
        ActivateViaRequestStationCancel,
        AccountCancelledSuccess,
        ChangePinSuccess,
        ChangePinFailed,
        ChangePinMaxReached,
        DeviceNotSupported,
        ActivateViaRequestStationCodeExpired,
        DeactivateAppSuccess,
        ActivationPendingNoSMSCheck, // Gebruiker heeft een account aangevraagd, maar dan zonder SMS controle
        InAppActivationCodeBlocked,
        InAppActivationCodeExpired,
        EmailRegistrationTooManyMails,
        EmailRegistrationMaximumReached,
        EmailRegistrationCodeBlocked, //te vaak foutieve code ingevoerd
        UpgradeAndAuthenticateFailed,
        EmailRegistrationAlreadyVerified,
        EmailRegistrationSuccess,
        EmailRegistrationSuccessfulVerified,
        AppDeactivated,
        NoValidDocuments,
        RequestAccountBlockedTemporarily,
        RequestAccountInvalid,
        RequestAccountNotAvailable,
        RequestAccountDeceased,
        RequestAccountTooOftenDay,
        RequestAccountTooOftenMonth,
        RequestAccountAccountBlocked,
        RequestAccountBRPTimeout,
        RequestAccountNotLatestAddress,
        RequestAccountAddressUnderInvestigating,
        RequestAccountAddressAbroad,
        UpgradeAndAuthenticateAborted,
        ClassifiedDeceased,
        ContactHelpDesk,
    }
}

