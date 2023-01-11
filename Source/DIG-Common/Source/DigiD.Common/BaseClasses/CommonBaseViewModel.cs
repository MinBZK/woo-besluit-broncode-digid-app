// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
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
