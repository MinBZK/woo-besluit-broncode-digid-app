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
﻿using DigiD.Common.BaseClasses;
using DigiD.Common.Services;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    public class VideoPlayerViewModel : CommonBaseViewModel
    {
        public MediaSource VideoSource { get; set; }
        private readonly string _transcript;

        public VideoPlayerViewModel(VideoFile video)
        {
            switch (video)
            {
                case VideoFile.Koppelcode:
                    PageId = "AP166";
                    VideoSource = MediaSource.FromUri("ms-appx:///vid_koppelcode.mp4");
                    _transcript = AppResources.AP166_Transcript;
                    break;
                case VideoFile.QrCode:
                    PageId = "AP165";
                    VideoSource = MediaSource.FromUri("ms-appx:///vid_qrcode.mp4");
                    _transcript = AppResources.AP165_Transcript;
                    break;
                case VideoFile.Rda:
                    PageId = "AP169";
                    VideoSource = MediaSource.FromUri("ms-appx:///vid_rda.mp4");
                    _transcript = Device.RuntimePlatform == Device.Android ? AppResources.AP169_Transcript_Android : AppResources.AP169_Transcript_iOS;
                    break;
            }
        }

        private bool _init;

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (_init)
                return;

            _init = true;

            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                DependencyService.Get<IA11YService>().Speak(_transcript, 500);
        }

        public AsyncCommand CloseCommand => new AsyncCommand(async () => await NavigationService.PopCurrentModalPage());
    }

    public enum VideoFile
    {
        Koppelcode,
        QrCode,
        Rda,
    }
}
