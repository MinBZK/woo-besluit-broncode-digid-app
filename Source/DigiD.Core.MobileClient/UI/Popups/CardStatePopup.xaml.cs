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
using System.Windows.Input;
using DigiD.Common.EID.Demo;
using DigiD.Common.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DemoHelper = DigiD.Common.EID.Demo.DemoHelper;

namespace DigiD.UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardStatePopup : DigiD.Common.BaseClasses.BasePopup
    {
        private int _tries;
        private bool _isSuspended;
        private bool _isBlocked;

        public bool ChangePinRequired { get; set; }
        
        public string PIN => DemoHelper.CardState.Value.PIN;
        public int Tries => IsSuspended ? 1 : IsBlocked ? 0 : _tries;
        
        public bool IsSuspended
        {
            get => _isSuspended;
            set
            {
                if (IsBlocked)
                    IsBlocked = false;

                _isSuspended = value;
                OnPropertyChanged(nameof(IsSuspended));
            }
        }
        
        public bool IsBlocked
        {
            get => _isBlocked;
            set
            {
                if (IsSuspended)
                    IsSuspended = false;

                _isBlocked = value;
                OnPropertyChanged(nameof(IsBlocked));
            }
        }

        public CardStatePopup()
        {
            var width = DisplayHelper.Width * .8;

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                width = 340;
            }
            Size = new Size(width, 300);

            InitializeComponent();
            BindingContext = this;

            _tries = DemoHelper.CardState.Value.PINTries;
            IsSuspended = DemoHelper.CardState.Value.IsSuspended;
            IsBlocked = DemoHelper.CardState.Value.IsBlocked;
            ChangePinRequired = DemoHelper.CardState.Value.ChangePinRequired;
            
            OnPropertyChanged(nameof(Tries));
        }

        protected override void LightDismiss()
        {
            base.LightDismiss();
            IsOpened = false;
            DemoHelper.CardState = new Lazy<CardState>(() => new CardState()
            {
                ChangePinRequired = ChangePinRequired,
                PINTries = Tries
            });
        }

        public ICommand ResetCommand => new Command(() =>
        {
            _tries = 5;
            ChangePinRequired = true;
            IsSuspended = false;
            IsBlocked = false;
        });
    }
}
