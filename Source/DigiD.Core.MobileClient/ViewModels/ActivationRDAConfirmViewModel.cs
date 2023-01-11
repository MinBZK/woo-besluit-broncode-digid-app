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
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.RDA.ViewModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class ActivationRdaConfirmViewModel : BaseViewModel
    {
#if A11YTEST
        public ActivationRdaConfirmViewModel() : this(new RdaSessionResponse(),
            async (r) => { await Task.FromResult(false); }, async () => { await Task.CompletedTask; })
        {

        }
#endif

        public ActivationRdaConfirmViewModel(RdaSessionResponse session, Func<bool, Task> completeAction, Func<Task> retryAction)
        {
            PageId = "AP086";
            HeaderText = AppResources.AP086_Header;
            FooterText = AppResources.AP086_Message;
            ButtonCommand = new AsyncCommand(async () =>
            {
                await DependencyService.Get<INavigationService>().PushAsync(new RdaViewModel(session, completeAction, "AP038", false, retryAction));
            });

            NavCloseCommand = new AsyncCommand(async () =>
            {
                var result = await DependencyService.Get<IAlertService>().DisplayAlert(
                    AppResources.ActivationAnnulerenAlertHeader
                    , AppResources.ActivationAnnulerenAlertMessage
                    , AppResources.Yes
                    , AppResources.No);

                if (result)
                {
                    if (HttpSession.ActivationSessionData?.ActivationMethod != ActivationMethod.RequestNewDigidAccount)
                        await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.ActivationCancelled));
                    else
                        await completeAction.Invoke(false);
                }
            });
        }
    }
}
