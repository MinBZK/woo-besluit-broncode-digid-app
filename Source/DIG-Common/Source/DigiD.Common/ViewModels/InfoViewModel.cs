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
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Enums;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    public class InfoViewModel : CommonBaseViewModel
    {
        public string Message { get; set; }
#if A11YTEST
        public InfoViewModel() : this(new HelpModel(InfoPageType.LoginHelp)) { }
#endif

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(HeaderText))
                DependencyService.Get<IA11YService>().Speak(string.Format(AppResources.InformationHelp, HeaderText));
        }

        public InfoViewModel(HelpModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                Message = string.Empty;
                var cardName = EIDSession.Card != null ? EIDSession.Card.DocumentType.Translate().ToLowerInvariant() : DocumentType.Generic.Translate().ToLowerInvariant();

                switch (model.InfoPageType)
                {
                    case InfoPageType.EmailEntry:
                        PageId = "AP167";
                        HeaderText = AppResources.AP167_Header;
                        Message = AppResources.AP167_Message;
                        break;
                    case InfoPageType.EmailConfirm:
                        PageId = "AP168";
                        HeaderText = AppResources.AP168_Header;
                        Message = AppResources.AP168_Message;
                        break;
                    case InfoPageType.LoginAndUpgrade:
                        HeaderText = AppResources.LoginAndUpgradeHelpHeader;
                        Message = AppResources.LoginAndUpgradeHelpMessage;
                        PageId = "AP151";
                        break;
                    case InfoPageType.UpgradeRDA:
                        PageId = "AP162";
                        Message = AppResources.RDAIntroMessage;
                        HeaderText = AppResources.RDAIntroHeader;
                        break;
                    case InfoPageType.PinActivated:
                        PageId = "AP155";
                        Message = AppResources.PINActivatedMessage;
                        HeaderText = AppResources.InformationTitleActivated;
                        break;
                    case InfoPageType.PinRegistration:
                        PageId = "AP150";
                        Message = AppResources.PINRegistrationMessage;
                        HeaderText = AppResources.InformationTitleRegistration;
                        break;
                    case InfoPageType.WIDHelpScannerInfo:
                        PageId = "AP154";
                        Message = string.Format(AppResources.WIDHelpScannerInfoMessage, cardName);
                        HeaderText = AppResources.WIDHelpScannerInfoHeader;
                        break;
                    case InfoPageType.WIDHelpEnterPIN:
                        Message = string.Format(AppResources.WIDHelpEnterPINMessage, cardName);
                        HeaderText = AppResources.WIDHelpEnterPINHeader;
                        PageId = "AP156";
                        break;
                    case InfoPageType.WIDHelpChangeTransportPIN:
                        PageId = "AP157";
                        Message = string.Format(AppResources.WIDHelpChangeTransportPINMessage, cardName);
                        HeaderText = AppResources.WIDHelpChangeTransportPINHeader;
                        break;
                    case InfoPageType.WIDHelpResumePIN:
                        PageId = "AP158";
                        Message = string.Format(AppResources.WIDHelpResumePINMessage, cardName, EIDSession.Card!.CANName, EIDSession.Card.CANLength);
                        HeaderText = AppResources.WIDHelpResumePINHeader;
                        break;
                    case InfoPageType.WIDHelpChangePIN_PIN:
                        PageId = "AP159";
                        Message = string.Format(AppResources.WIDHelpChangePIN_PINMessage, cardName);
                        HeaderText = AppResources.WIDHelpChangePIN_PINHeader;
                        break;
                    case InfoPageType.LoginHelp:
                        Message = AppResources.LoginHelpMessage;
                        HeaderText = AppResources.ActivationHelpHeader;
                        PageId = "AP153";
                        break;
                    case InfoPageType.KoppelcodeHelp:
                        HeaderText = AppResources.KoppelcodeHelpHeader;
                        Message = AppResources.KoppelcodeHelpMessage;
                        PageId = "AP161";
                        break;
                    default:
                        {
                            Message = $"Resource not found for {model.InfoPageType}";
                            HeaderText = $"Resource not found for {model.InfoPageType}";
                            break;
                        }
                }

                PiwikHelper.TrackView($"Help: {PageId} - {HeaderText}", GetType().FullName);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string>()
                {
                    {"PageId", PageId},
                    {"Model",JsonConvert.SerializeObject(model)}
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;
                    await NavigationService.PopCurrentModalPage();
                    CanExecute = true;

                }, () => CanExecute);
            }
        }

        public ICommand OpenWebsite
        {
            get
            {
                return new AsyncCommand<string>(async (url) =>
                   {
                       if (!CanExecute)
                           return;

                       CanExecute = false;

                       switch (url.ToString())
                       {
                           case "FAQ":
                               await Launcher.OpenAsync(new Uri(AppResources.FAQLink));
                               break;
                           case "TermsOfUse":
                               await Launcher.OpenAsync(new Uri(AppResources.TermsOfUseLink));
                               break;
                           case "PrivacyPolicy":
                               await Launcher.OpenAsync(new Uri(AppResources.PrivacyPolicyLink));
                               break;
                       }
                       CanExecute = true;

                   }, (u) => CanExecute);
            }
        }
    }
}

