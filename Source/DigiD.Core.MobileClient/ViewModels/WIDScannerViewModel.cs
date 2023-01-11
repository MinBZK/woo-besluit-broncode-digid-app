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
using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class WidScannerViewModel : BaseNfcScannerViewModel
    {
        public bool NFCDisabled { get; set; }

#if A11YTEST
        public WidScannerViewModel() : this(new NfcScannerModel { DocumentType = DocumentType.DrivingLicense }) { }
#endif

        public bool IsBlurVisible { get; set; }

        internal WidScannerViewModel(NfcScannerModel model) : base(model)
        {
            HeaderText = AppResources.WIDScannerMobileHeader;
            NFCDisabled = !NfcService.IsNFCEnabled;

            if (NFCDisabled)
            {
                AnimationVisible = false;
                ImageSource = "resource://DigiD.Common.Resources.afbeelding_aanzetten_nfc.svg?assembly=DigiD.Common";
            }
            else
            {
                ImageSource = "resource://DigiD.Common.Resources.afbeelding_read_document.svg?assembly=DigiD.Common";
            }

            var startScan = model.EIDSessionResponse?.Host != null && new Uri(model.EIDSessionResponse.Host).Host.StartsWith("eid");

            if (!NFCDisabled)
            {
                PageId = "AP038";
                FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDScannerMobileFooter, DocumentName);
                AnimationSource = AppResources.NFC_RDA_Animation_RemoveFromCover;
                AnimationVisible = true;
                startScan = true;
            }
            else
            {
                HeaderText = AppResources.AP037_Header;
                FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.AP037_Message, DocumentName);
                PageId = "AP037";
            }

            if (startScan)
            {
                Device.BeginInvokeOnMainThread(async () => await StartScan(false));
            }

            CancelCommand = new AsyncCommand(async () =>
            {
                if (!CanExecute)
                    return;

                CanExecute = false;

                await Cancel();

                if (Model.Action == PinEntryType.Authentication || Model.Action == PinEntryType.ChangeTransportPIN)
                {
                    await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                    await NavigationService.ShowMessagePage(MessagePageType.WIDCancelled);
                }
                else
                    await NavigationService.PopToRoot();

                CanExecute = true;
            }, () => CanExecute);

            NavCloseCommand = CancelCommand;
        }

        public AsyncCommand NextCommand { get; set; }
        protected bool IsNextCommandExecuted;

        public override async Task ReadPhotoCompleted(byte[] photo)
        {
            await NavigationService.PushAsync(new WidPhotoViewModel(photo));
        }

        public override async Task RandomizationComplete()
        {
            if (HttpSession.IsWeb2AppSession)
            {
                if (DependencyService.Get<IDemoSettings>().IsDemo)
                    await NavigationService.ShowMessagePage(MessagePageType.LoginSuccess);
                else
                {
                    IsBlurVisible = true;
                    await DependencyService.Get<IA11YService>().Speak(AppResources.LoginSuccessMessage);
                    
                    NextCommand = new AsyncCommand(async () =>
                    {
                        IsNextCommandExecuted = true;
                        App.ShowSplashScreen(true, true);
                        await AuthenticationHelper.ProcessWebAuthentication(Model.EIDSessionResponse.ReturnURL);
                        await NavigationService.PopToRoot(true);
                        App.ClearSession();
                    }, () => !IsNextCommandExecuted);

                    return;
                }
            }
            else
            {
                await NavigationService.ShowMessagePage(MessagePageType.LoginSuccess);
            }

            App.ClearSession();
        }

        public Command EnableNFCCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;
                    NfcService.OpenNFCSettings();
                    CanExecute = true;
                });
            }
        }
    }
}
