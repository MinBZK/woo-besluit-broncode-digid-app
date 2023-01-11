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
﻿using DigiD.Common.Constants;
using DigiD.Common.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
#if DEBUG
using System.Diagnostics;
#endif
using System.Windows.Input;
using DigiD.Common.Enums;
using DigiD.Common.SessionModels;
using Xamarin.Forms;

namespace DigiD.Common.BaseClasses
{
    public class CommonBaseViewModel : BindableObject, IViewModel
    {
        #region Accessibility
        public static bool IsInVoiceOverMode => DependencyService.Get<IA11YService>().IsInVoiceOverMode();
        public static bool IsNotInVoiceOverMode => !DependencyService.Get<IA11YService>().IsInVoiceOverMode();
        // Onderstaande 3 methoden zijn voor accessibility.
        public virtual string PageIntroName => $"{HeaderText}";
        public virtual string PageIntroHelp => $"{HeaderText} {FooterText}";
        #endregion

        public bool IsDemoMode => AppSession.AppMode == AppMode.Demo && ShowDemoBar;
        public bool ShowDemoBar { get; protected set; } = true;
        public Command DemoBarPressed { get; protected set; }

        public bool PreventLock { get; set; }
        public string PageId { get; protected set; } = PiwikConstants.TBD;
        public bool HasBackButton { get; set; }
        public bool IsError { get; set; }
        public string HeaderText { get; set; }
        public string FooterText
        {
            get => _footerText;
            set
            {
                _footerText = value;
                OnPropertyChanged(nameof(FooterText));
                AccessibilityFooterText = value;
            }
        }


        public string AccessibilityFooterText
        {
            get => _accessibilityFooterText;
            set
            {
                _accessibilityFooterText = string.Concat(value, (IsError ? $", {AppResources.AccessibilityTryAgain}" : string.Empty));
                OnPropertyChanged(nameof(AccessibilityFooterText));
            }
        }

        public ICommand ButtonCommand { get; set; }
        public string ButtonText { get; set; }
        public bool ButtonVisible => ButtonCommand != null;

        public ICommand NavCloseCommand { get; protected set; }
        public bool NavCloseButtonVisible => NavCloseCommand != null;

        protected INavigationService NavigationService { get; } = DependencyService.Get<INavigationService>();
        protected IDialog DialogService { get; } = DependencyService.Get<IDialog>();
        private string _footerText;
        private string _accessibilityFooterText;
        private readonly bool _disableLogging;

        public CommonBaseViewModel(bool disableLogging = false, bool preventLock = false)
        {
            _disableLogging = disableLogging;
            PreventLock = preventLock;
            
            CanExecute = true;
            IsError = false;
#if A11YTEST
            HasBackButton = true;
#endif
        }

        protected void TrackView()
        {
#if TEST || DEBUG
            DependencyService.Get<IDesktopService>()
                ?.SetTitle(Device.RuntimePlatform == Device.UWP ? $"{PageId}" : $"DigiD - {PageId}");
#endif
            PiwikHelper.TrackView($"{PageId} - {HeaderText}", GetType().FullName);
        }

        public virtual void OnAppearing()
        {
            if (_disableLogging)
                return;

            TrackView();
#if DEBUG
            if (PageId.Contains(PiwikConstants.TBD))
            {
                Debug.WriteLine("\n\n*** PIWIK **********************\n\n" +
                                                   $"\tmodel:  '{GetType().FullName}'" +
                                                   $"\tPageId: '{PageId}'" +
                                                   "\n\n*** PIWIK **********************\n\n");
            }
#endif
        }

        public virtual void OnDisappearing()
        {

        }

        public virtual bool OnBackButtonPressed()
        {
            return !HasBackButton;
        }

        protected bool CanExecute { get; set; }
    }
}
