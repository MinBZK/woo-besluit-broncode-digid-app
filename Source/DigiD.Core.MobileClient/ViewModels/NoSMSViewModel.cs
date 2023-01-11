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
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using Microsoft.AppCenter.Crashes;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class NoSmsViewModel : BaseViewModel
    {
        private int _seconds = -1;

#if A11YTEST
        public NoSmsViewModel( ) : this("0611223344")
        {
            FooterText = "Alleen deze melding mag getoond worden.";
            IsError = true;
        }
#endif

        public NoSmsViewModel(string mobileNumber)
        {
            PageId = "AP047";
            HeaderText = AppResources.NoSMSHeader;
            FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.NoSMSReceivedMessage2, mobileNumber);
            HasBackButton = true;
        }

        public AsyncCommand ResendSpokenSMSCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    await ResendSms(true);
                }, () => _seconds < 0);
            }
        }

        public AsyncCommand ResendSMSCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    await ResendSms(false);
                }, () => _seconds < 0);
            }
        }


        private async Task ResendSms(bool spoken)
        {
            _seconds = -1;

            var result = await DependencyService.Get<IEnrollmentServices>().ResendSMS(spoken);
            IsError = false;

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        await NavigationService.GoBack();
                        break;
                    }
                case ApiResult.Nok:
                    {
                        IsError = true;
                        switch (result.ErrorMessage)
                        {
                            case "sms_too_fast":
                                try
                                {
                                    _seconds = (int)result.Payload.time_left;

                                    while (_seconds > 0)
                                    {
                                        FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.SingleDeviceSMSTooFast, _seconds);
                                        await Task.Delay(1000);
                                        _seconds--;
                                    }
                                    if (_seconds == 0)
                                    {
                                        FooterText = AppResources.SingleDeviceSMSRetry;
                                        _seconds = -1;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Crashes.TrackError(e, new Dictionary<string, string>
                                    {
                                        {"Error",result.ErrorMessage},
                                        {"Payload",result.Payload ?? "unknown"}
                                    });
                                }

                                return;
                        }
                        break;
                    }
                case ApiResult.HostNotReachable:
                    {
                        IsError = true;
                        FooterText = AppResources.NoInternetConnectionMessage;
                        break;
                    }
                case ApiResult.Unknown:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.LoginUnknown);
                        break;
                    }
                case ApiResult.SessionNotFound:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                        break;
                    }
                case ApiResult.SSLPinningError:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                        break;
                    }
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    break;
            }
        }

        public override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
