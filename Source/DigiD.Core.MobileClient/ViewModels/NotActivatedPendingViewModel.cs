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
﻿using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DigiD.ViewModels
{
    public class NotActivatedPendingViewModel : BaseViewModel
    {
        private readonly App2AppSessionData _data;
        public bool IsApp2App { get; set; }

#if A11YTEST
        public NotActivatedPendingViewModel() : this(null) { }
#endif
        public NotActivatedPendingViewModel(App2AppSessionData data)
        {
            IsApp2App = data != null;
            _data = data;
            
            HeaderText = IsApp2App ? AppResources.NotActivatedHeader : AppResources.NotActivatedPendingHeader;
            PageId = IsApp2App ? "AP021" : "AP022";

            ButtonCommand = new AsyncCommand(async() =>
            {
                CanExecute = false;
                App.ClearSession();

                DialogService.ShowProgressDialog();

                await ActivationHelper.ContinueLetterActivation();
                    
                DialogService.HideProgressDialog();

                CanExecute = true;
            }, () => CanExecute);
        }

        public ICommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;
                    DialogService.ShowProgressDialog();
                    await IncomingDataHelper.ProcessData(_data, false, false);
                    await NavigationService.PopToRoot(true);
                    DialogService.HideProgressDialog();
                    CanExecute = true;
                }, () => CanExecute);
            }
        }
    }
}
