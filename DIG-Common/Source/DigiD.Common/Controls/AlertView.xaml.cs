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
using DigiD.Common.Enums;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public partial class AlertView : Popup
    {
        public string HeaderText { get; set; }
        public string MessageText { get; set; }
        public string AcceptText { get; set; }
        public string CancelText { get; set; }
        public bool TwoButtonsVisible { get; set; }
        public bool SingleButtonVisible => !TwoButtonsVisible;
        
        public AlertView(string title, string message, string accept, string cancel, bool invertCallToAction = false)
        {
            InitializeComponent();

            BindingContext = this;

            AcceptText = accept;
            CancelText = cancel;
            HeaderText = title;
            MessageText = message;

            if (invertCallToAction)
            {
                btnAccept.ButtonType = ButtonType.Secundairy;
                btnCancel.ButtonType = ButtonType.Primary;
            }

            TwoButtonsVisible = !string.IsNullOrEmpty(cancel);

            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            container.SizeChanged += Layout_SizeChanged;
        }
        protected override void LightDismiss()
        {
            DeviceDisplay.MainDisplayInfoChanged -= DeviceDisplay_MainDisplayInfoChanged;
            base.LightDismiss();
        }
        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            var isLandscape = e.DisplayInfo.Orientation == DisplayOrientation.Landscape;
            var height = Math.Min(container.Height, isLandscape ? DisplayHelper.Height - 40 : DisplayHelper.Height * .8);
            Resize(height, true);
        }

        private void ClosePopup(object result)
        {
            container.SizeChanged -= Layout_SizeChanged;
            DeviceDisplay.MainDisplayInfoChanged -= DeviceDisplay_MainDisplayInfoChanged;
            Dismiss(result);
        }

        protected override object GetLightDismissResult()
        {
            return false;
        }

        void BorderedButton_OkClicked(object sender, EventArgs e)
        {
            ClosePopup(true);
        }

        void BorderedButton_CancelClicked(object sender, EventArgs e)
        {
            ClosePopup(false);
        }

        void Layout_SizeChanged(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var height = container.Height;
                Resize(height);
            });
        }

        private void Resize(double height, bool orientationChanged = false)
        {
            if (height == popupFrame.HeightRequest && !orientationChanged)
                return;

            var currentOrientation = DeviceDisplay.MainDisplayInfo.Orientation;
            if (currentOrientation == DisplayOrientation.Landscape)
            {
                popupFrame.WidthRequest = DisplayHelper.Width * .8;
                scrollView.WidthRequest = DisplayHelper.Width * .8 - popupFrame.Padding.HorizontalThickness;
            }
            else
            {
                popupFrame.WidthRequest = DisplayHelper.Width - 40;
                scrollView.WidthRequest = DisplayHelper.Width - 40 - popupFrame.Padding.HorizontalThickness;
            }
            popupFrame.HeightRequest = height + popupFrame.Padding.VerticalThickness;
            scrollView.HeightRequest = height;

            WidthRequest = popupFrame.WidthRequest;     // Device.RuntimePlatform == Device.Android ? popupFrame.WidthRequest : DisplayHelper.Width;
            HeightRequest = popupFrame.HeightRequest;   // Device.RuntimePlatform == Device.Android ? popupFrame.HeightRequest : DisplayHelper.Height;

            //var dbString = $"=====> AlertView.Resize(height: '{height}', orientationChanged: '{orientationChanged}') -- popupFrame.Size (w / h): '{popupFrame.Width}' / '{popupFrame.Height}' -- orientation: '{currentOrientation}' -- scrollView.WidthRequest: '{scrollView.WidthRequest}'";

            Size = new Xamarin.Forms.Size(WidthRequest, HeightRequest);
        }
    }
}
