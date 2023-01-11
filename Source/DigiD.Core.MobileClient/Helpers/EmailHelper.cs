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
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Interfaces;
using DigiD.Models;
using DigiD.ViewModels;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    internal static class EmailHelper
    {
        internal static async Task<CheckEmailModel> CheckEmailStatus()
        {
            var response = await DependencyService.Get<IEmailService>().Status();
            var result = new CheckEmailModel();
            var tcs = new TaskCompletionSource<CheckEmailModel>();

            if (response.UserActionNeeded)
            {
                switch (response.EmailStatus)
                {
                    case EmailStatus.Verified:
                        {
                            await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.ConfirmEmailAddress, response.EmailAddress), false, async isConfirmed =>
                            {
                                await DependencyService.Get<IEmailService>().Confirm(isConfirmed);
                                result.Action = ConfirmActions.ConfirmEmailAddress;
                                result.Confirmed = isConfirmed;
                                tcs.TrySetResult(result);
                            }));
                            break;
                        }
                    case EmailStatus.NotVerified:
                        {
                            await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.ConfirmEmailAddress, response.NoVerifiedEmailAddress), false, async isConfirmed =>
                            {
                                await Task.Delay(0);
                                result.Action = ConfirmActions.ConfirmExistingEmailAddress;
                                result.EmailAddress = response.NoVerifiedEmailAddress;
                                result.Confirmed = isConfirmed;
                                tcs.TrySetResult(result);
                            }));
                            break;
                        }
                    case EmailStatus.None:
                        {
                            await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RegisterEmailAddress), false, async isValid =>
                            {
                                await Task.Delay(0);
                                result.Action = ConfirmActions.RegisterEmailAddress;
                                result.Confirmed = isValid;
                                tcs.TrySetResult(result);
                            }));
                            break;
                        }
                }
            }
            else
                tcs.SetResult(null);

            return await tcs.Task;
        }

        internal static async Task Manage(bool fromLanding)
        {
            var emailStatus = await DependencyService.Get<IEmailService>().Status();

            if (emailStatus.EmailStatus == EmailStatus.None)
            {
                await DependencyService.Get<INavigationService>().PushAsync(new EmailRegisterViewModel(new RegisterEmailModel
                {
                    AbortAction = async () =>
                    {
                        if (fromLanding)
                            await DependencyService.Get<INavigationService>().ResetMainPage(new AP087ViewModel());
                        else
                            await DependencyService.Get<INavigationService>().ResetMainPage(new AP106ViewModel());
                    },
                    NextAction = async () => await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.EmailRegistrationSuccess)
                }));
            }
            else
            {
                var model = new ConfirmModel(ConfirmActions.ConfirmExistingEmailAddress, emailStatus.EmailStatus == EmailStatus.Verified ? emailStatus.EmailAddress : emailStatus.NoVerifiedEmailAddress);
                await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(model, true,
                    async confirmResult =>
                    {
                        if (confirmResult)
                        {
                            if (emailStatus.EmailStatus == EmailStatus.Verified)
                            {
                                await DependencyService.Get<IEmailService>().Confirm(true);
                                await DependencyService.Get<INavigationService>().GoBack();
                            }
                            else
                                await DependencyService.Get<INavigationService>().PushAsync(new EmailConfirmViewModel(new RegisterEmailModel(true)
                                    {
                                        NextAction = async () =>
                                            await DependencyService.Get<INavigationService>()
                                                .ShowMessagePage(MessagePageType.EmailRegistrationSuccessfulVerified),
                                        AbortAction = async () =>
                                        {
                                            if (fromLanding)
                                                await DependencyService.Get<INavigationService>()
                                                    .ResetMainPage(new AP087ViewModel());
                                            else
                                                await DependencyService.Get<INavigationService>()
                                                    .ResetMainPage(new AP106ViewModel());
                                        }
                                    }, emailStatus.EmailAddress ?? emailStatus.NoVerifiedEmailAddress, false));
                        }
                        else
                            await DependencyService.Get<INavigationService>().PushAsync(new EmailRegisterViewModel(
                                new RegisterEmailModel
                                {
                                    AbortAction = async () =>
                                    {
                                        if (fromLanding)
                                            await DependencyService.Get<INavigationService>()
                                                .ResetMainPage(new AP087ViewModel());
                                        else
                                            await DependencyService.Get<INavigationService>()
                                                .ResetMainPage(new AP106ViewModel());
                                    },
                                    NextAction = async () =>
                                        await DependencyService.Get<INavigationService>()
                                            .ShowMessagePage(MessagePageType.EmailRegistrationSuccess)
                                }));
                    }));
            }
        }
    }
}
