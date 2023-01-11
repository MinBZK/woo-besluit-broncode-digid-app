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
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class QRScannerViewModel : BaseViewModel
    {
#if A11YTEST
        public QRScannerViewModel() : this(false, true) { }
#endif

        public QRScannerViewModel(bool eIDAuthentication, bool canGoBack)
        {
            HasBackButton = canGoBack;
            PageId = "AP014";
            FooterText = AppResources.ScannerFooter;

            NavCloseCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot();
            });

            if (eIDAuthentication)
                HeaderText = AppResources.WIDQRScannerHeader;
            else
            {
                switch (DependencyService.Get<IMobileSettings>().ActivationStatus)
                {
                    case ActivationStatus.Activated:
                        HeaderText = AppResources.ScannerHeaderAuthenticate;
                        break;
                    case ActivationStatus.Pending:
                        HeaderText = AppResources.ScannerHeaderPending;
                        break;
                    case ActivationStatus.NotActivated:
                        HeaderText = AppResources.ScannerHeaderActivation;
                        break;
                }
            }
        }
        
        public AsyncCommand<string> ScanResultCommand
        {
            get
            {
                return new AsyncCommand<string>(async data =>
                {
                    if (!CanExecute)
                        return;
                    
                    Xamarin.Essentials.Vibration.Vibrate(100);
                    CanExecute = false;

                    DialogService.ShowProgressDialog();

                    var d = data.Replace("://", ":");

                    if (Uri.TryCreate(d, UriKind.Absolute, out var uri))
                    {
                        HttpSession.Browser = Browser.Unknown;
                        await IncomingDataHelper.ScanCompletedAsync(uri, false);
                    }   
                    else
                        await NavigationService.ShowMessagePage(MessagePageType.InvalidScan);

                    DialogService.HideProgressDialog();

                    CanExecute = true;
                }, d => CanExecute);
            }
        }

        public AsyncCommand HelpCommand => new AsyncCommand(async () =>
        {
            await NavigationService.PushModalAsync(new VideoPlayerViewModel(VideoFile.QrCode), true, false);
        });
    }
}

