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
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using DigiD.Helpers;
using DigiD.Models;
using PropertyChanged;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class WhatsNewTourViewModel : BaseViewModel
    {
        private WhatsNewPageModel _currentItem;

        [DependsOn(nameof(CurrentPageItem))]
        public int Index { get; private set; }

        [DependsOn(nameof(CurrentPageItem))]
        public bool HasPreviousPage => Index > 0;

        [DependsOn(nameof(CurrentPageItem))]
        public bool HasNextPage => Index + 1 < WhatsNewPages.Count;

        [DependsOn(nameof(CurrentPageItem))]
        public ICommand PreviousPageCommand { get; }

        [DependsOn(nameof(CurrentPageItem))]
        public ICommand NextPageCommand { get; }

        [DependsOn(nameof(CurrentPageItem))]
        public string PageInfo => string.Format(CultureInfo.InvariantCulture, AppResources.WhatsNewPageInfo, Index + 1, WhatsNewPages.Count);

        [AlsoNotifyFor(nameof(HasNextPage), nameof(HasPreviousPage))]
        public WhatsNewPageModel CurrentPageItem
        {
            get
            {
                if (_currentItem == null)
                {
                    Index = 0;
                    _currentItem = WhatsNewPages[Index];
                }
                return _currentItem;
            }
            set
            {
                _currentItem = value;
                var index = WhatsNewPages.IndexOf(value);
                if (index >= 0)
                    Index = index;

                OnPropertyChanged(nameof(CurrentPageItem));
            }
        }

        public List<WhatsNewPageModel> WhatsNewPages { get; set; }
        public ButtonType NextButtonType => HasNextPage ? ButtonType.Secundairy : ButtonType.Primary;
        public string NextButtonText => HasNextPage ? AppResources.Next : AppResources.WhatsNewSluitenButtonText;
        public bool ShowPageIndicator => WhatsNewPages.Count > 1 && !DependencyService.Get<IA11YService>().IsInVoiceOverMode();

#if A11YTEST
        public WhatsNewTourViewModel() : this (null) { }
#endif

        public WhatsNewTourViewModel(Func<Task> stopAction = null)
        {
            PageId = "AP078";

            WhatsNewPages = new List<WhatsNewPageModel>(WhatsNewHelper.Data.GetPages());

            if (!string.IsNullOrEmpty(WhatsNewHelper.Data.Version))
                DependencyService.Get<IGeneralPreferences>().WhatsNewVersion = WhatsNewHelper.Data.Version;

            ButtonCommand = new AsyncCommand(async () =>
            {
                if (stopAction == null)
                    await DependencyService.Get<INavigationService>().PopCurrentModalPage(false);
                else
                    await stopAction.Invoke();
            });

            PreviousPageCommand = new Command(() =>
            {
                if (Index > 0)
                {
                    Index -= 1;
                    CurrentPageItem = WhatsNewPages[Index];
                }
            }, () => HasPreviousPage);

            NextPageCommand = new Command(() =>
            {
                if (HasNextPage)
                {
                    Index += 1;
                    CurrentPageItem = WhatsNewPages[Index];
                }
                else
                {
                    if (ButtonCommand.CanExecute(null))
                        ButtonCommand.Execute(null);
                }
            });
        }
        public override string PageIntroName => $"{PageInfo}, {CurrentPageItem.Title}, {CurrentPageItem.Text}" +
            $", {(CurrentPageItem.IsAnimation ? CurrentPageItem.AlternateText : "")}";

    }
}

