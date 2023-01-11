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
﻿using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.SessionModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class VerificationCodeEntryViewModel : BaseViewModel
    {
        public bool IsValid => VerificationCode.Length == 4;
        public VerificationCodeEntryViewModel()
        {
            HasBackButton = true;

            PageId = "AP052";
            VerificationCode = string.Empty;
            HeaderText = AppResources.AP82Header;
            FooterText = AppResources.AP82Footer;
            NavCloseCommand = CloseCommand;
        }

        public string VerificationCode { get; set; }

        public AsyncCommand StartCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;

                    if (!await DependencyService.Get<IGeneralServices>().SslPinningCheck())
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                        return;
                    }

                    App.RegisterServices(VerificationCode == "DEMO" ? AppMode.Demo : AppMode.Normal);

                    DialogService.ShowProgressDialog();

                    var sessionResponse =
                        await DependencyService.Get<IEnrollmentServices>().InitSessionActivationByApp();

                    if (sessionResponse.ApiResult == ApiResult.Ok)
                    {
                        sessionResponse.VerificationCode = VerificationCode;
                        AppSession.Process = Process.AppActivationWithApp;

                        HttpSession.ActivationSessionData = new ActivationSessionData
                        {
                            ActivationMethod = ActivationMethod.App,
                        };

                        await NavigationService.PushAsync(new ActivationByAppViewModel(sessionResponse));
                    }
                    else
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.ErrorOccoured);
                    }

                    DialogService.HideProgressDialog();
                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        public AsyncCommand CloseCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (HttpSession.TempSessionData != null)
                    {
                        HttpSession.TempSessionData.SwitchSession();
                        HttpSession.TempSessionData = null;
                        await DependencyService.Get<IGeneralServices>().Cancel(true);
                    }

                    await App.CancelSession(true, async () => await NavigationService.PopToRoot(true));
                });
            }
        }

        public override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
