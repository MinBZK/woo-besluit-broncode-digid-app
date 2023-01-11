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
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.ViewModels;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
	public partial class HelpButtonView : Grid
    {
        public static readonly BindableProperty InfoPageTypeProperty = BindableProperty.Create(nameof(InfoPageType), typeof(InfoPageType), typeof(HelpButtonView), InfoPageType.Undefined);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HelpButtonView), null, BindingMode.OneTime);

        public InfoPageType InfoPageType
	    {
	        get => (InfoPageType) GetValue(InfoPageTypeProperty);
	        set => SetValue(InfoPageTypeProperty, value);
	    }
        
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public HelpButtonView()
		{
			InitializeComponent ();
        }

        protected override void OnTabIndexPropertyChanged(int oldValue, int newValue)
        {
            base.OnTabIndexPropertyChanged(oldValue, newValue);
            if (__HelpButton__ != null)
                __HelpButton__.TabIndex = newValue;
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            if (InfoPageType != InfoPageType.Undefined)
            {
                var infoViewModel = new InfoViewModel(new Models.HelpModel(InfoPageType));
                Device.BeginInvokeOnMainThread(async () =>  await DependencyService.Get<IPopupService>().OpenPopup(infoViewModel));
            }

            if (Command != null && Command.CanExecute(null))
                Command.Execute(null);
        }
    }
}
