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
