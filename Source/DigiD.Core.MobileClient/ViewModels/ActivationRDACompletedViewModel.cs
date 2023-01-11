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
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Common.SessionModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DigiD.ViewModels
{
    public class ActivationRdaCompletedViewModel : BaseViewModel
    {
        private readonly RegisterEmailModel _model;
        public string Header2Text { get; }
        public bool ShowPossibleIds => _model.ActivationMethod == ActivationMethod.RDA;

#if A11YTEST
        public ActivationRdaCompletedViewModel() : this(new RegisterEmailModel(Common.Enums.ActivationMethod.RDA, true)) { }
#endif

        public ActivationRdaCompletedViewModel(RegisterEmailModel model)
        {
            _model = model;
            PageId = $"AP007 - {model.ActivationMethod}";
            Header2Text = model.ActivationMethod == ActivationMethod.AccountAndApp ? AppResources.AP086Header3 : AppResources.AP086Header2;

            //Clear activationdata
            HttpSession.ActivationSessionData = null;
        }

        public new AsyncCommand ButtonCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;
                    await RdaHelper.Init(false);
                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;

                    await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.IDCheckPostponeAppActivation), true));
                    CanExecute = true;
                }, () => CanExecute);
            }
        }

    }
}
