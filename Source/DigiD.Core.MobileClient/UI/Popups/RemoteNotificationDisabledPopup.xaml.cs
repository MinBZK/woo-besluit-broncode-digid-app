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
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RemoteNotificationDisabledPopup : DigiD.Common.BaseClasses.BasePopup
    {
        public RemoteNotificationDisabledPopup()
        {
            var isInVoiceOvermode = DependencyService.Get<IA11YService>().IsInVoiceOverMode();

            var height = DisplayHelper.Height * (isInVoiceOvermode ? .9 : .8);
            var width = DisplayHelper.Width * (isInVoiceOvermode ? .9 : .8);

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                width = 340;
            }
            Size = new Size(width,height);

            InitializeComponent();
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            Dismiss(null);
            IsOpened = false;
        }

        protected override void LightDismiss()
        {
            base.LightDismiss();
            IsOpened = false;
        }


        private void OpenSettingsButton_OnClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IDevice>().OpenSettings();
        }
    }
}
