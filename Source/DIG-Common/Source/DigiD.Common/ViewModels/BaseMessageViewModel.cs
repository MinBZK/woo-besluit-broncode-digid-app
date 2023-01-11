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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Enums;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class BaseMessageViewModel : CommonBaseViewModel
    {
        public static dynamic Payload { get; set; }
        public string ImageSource { get; set; }
        private readonly MessagePageType _pageType;

        public override string PageIntroHelp => $"{HeaderText} {FooterText} {ButtonText}";

        public string AlternateTextImage { get; private set; } = "";

        protected static string CardName => EIDSession.Card != null ? EIDSession.Card.DocumentType.Translate().ToLowerInvariant() : DocumentType.Generic.Translate().ToLowerInvariant();

        protected virtual Task ChangePIN()
        {
            return Task.CompletedTask;
        }

        public BaseMessageViewModel(MessagePageType pageType)
        {
            _pageType = pageType;

            switch (pageType)
            {
                case MessagePageType.ErrorOccoured:
                case MessagePageType.ChallengeFailed:
                    {
                        PageId = "AP219";
                        SetData(AppResources.ErrorOccouredHeader, AppResources.ErrorOccouredMessage, AppResources.ErrorOccouredAction, false);
                        break;
                    }
                case MessagePageType.InvalidScan:
                    {
                        PageId = "AP211";
                        SetData(AppResources.InvalidScanHeader, AppResources.InvalidScanMessage, AppResources.InvalidScanAction, false);
                        break;
                    }
                case MessagePageType.ScanTimeout:
                    {
                        PageId = "AP212";
                        SetData(AppResources.ScanTimeoutHeader, AppResources.ScanTimeoutMessage, AppResources.ScanTimeoutAction, false);
                        break;
                    }
                case MessagePageType.SessionTimeout:
                    {
                        PageId = "AP207";
                        SetData(AppResources.SessionTimeoutHeader, AppResources.SessionTimeoutMessage, AppResources.OK, false);
                        break;
                    }
                case MessagePageType.HostUnknown:
                    {
                        PageId = "AP213";
                        SetData(AppResources.HostUnknownHeader, AppResources.HostUnkownMessage, AppResources.HostUnkownAction, false);
                        return;
                    }
                case MessagePageType.NetworkTimeout:
                    {
                        PageId = "AP215";
                        SetData(AppResources.TimeoutHeader, AppResources.TimeoutMessage, AppResources.TimeoutCommand, false);
                        break;
                    }
                case MessagePageType.SSLException:
                    {
                        PageId = "AP220";
                        SetData(AppResources.SSLWarningHeader, AppResources.SSLWarningMessage, AppResources.SSLWarningAction, false);
                        break;
                    }
                case MessagePageType.WIDCancelled:
                    {
                        PageId = "AP409";
                        SetData(pageType, null);
                        break;
                    }
                case MessagePageType.WIDFailed:
                    {
                        PageId = "AP245";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDActivationNeeded:
                    {
                        PageId = "AP243";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDLoginFailed:
                    {
                        PageId = "AP244";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.VerificationCodeFailed:
                    {
                        PageId = "AP214";
                        SetData(AppResources.VerificationCodeFailedHeader, AppResources.VerificationCodeFailedMessage, AppResources.VerificationCodeFailedButton, false);
                        break;
                    }
                case MessagePageType.WIDActivationFailed:
                    {
                        PageId = "AP255";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDReActivationFailed:
                    {
                        PageId = "AP273";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDReActivationSuccess:
                    {
                        PageId = "AP274";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDNotSupported:
                case MessagePageType.WIDRandomizeFailed:
                    {
                        PageId = "AP241";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDPUKBlocked:
                    {
                        PageId = "AP246";
                        SetData(pageType);
                        break;
                    }
                case MessagePageType.WIDResumePINSuccess:
                    {
                        PageId = "AP424";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDResumePUKSuccess:
                    {
                        PageId = "AP425";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDActivationSuccess:
                    {
                        PageId = "AP403";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDRetractSuccess:
                    {
                        PageId = "AP417";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDChangePINSuccess:
                    {
                        PageId = Device.Idiom == TargetIdiom.Desktop ? "DA012" : "AP412";
                        SetData(pageType, true, command: new AsyncCommand(async () =>
                        {
                            await NavigationService.PopToRoot(true);
                        }));
                        break;
                    }
                case MessagePageType.WIDRandomizeSuccess:
                    {
                        PageId = "AP999";
                        SetData(pageType, true);
                        break;
                    }
                case MessagePageType.WIDSuspended:
                    SetData(pageType, command: new AsyncCommand(async () => await ChangePIN()));
                    break;
                case MessagePageType.WIDPINBlocked:
                    {
                        PageId = Device.Idiom == TargetIdiom.Desktop ? "DA116" : "AP240";
                        SetData(string.Format(CultureInfo.InvariantCulture, AppResources.WIDPINBlockedHeader, CardName),
                            string.Format(CultureInfo.InvariantCulture, AppResources.AP240_Message, CardName), AppResources.OK, false);
                        break;
                    }
                case MessagePageType.WIDFirstUse:
                    {
                        PageId = "AP243";
                        SetData(MessagePageType.WIDFirstUse, false, new AsyncCommand(
                            async () =>
                            {
                                await Launcher.OpenAsync(new Uri("https://mijn.digid.nl/inlogmethoden"));
                            }));
                        break;
                    }
                case MessagePageType.UnknownError:
                    {
                        PageId = "AP252";
                        SetData(MessagePageType.UnknownError);
                        break;
                    }
            }
        }

        public void SetData(MessagePageType type, bool? success = false, ICommand command = null)
        {
            try
            {
                var header = string.Format(CultureInfo.InvariantCulture, TextResourceHelper.GetTextResource($"{type}Header"), CardName);
                var message = string.Format(CultureInfo.InvariantCulture, TextResourceHelper.GetTextResource($"{type}Message"), CardName);
                var button = string.Format(CultureInfo.InvariantCulture, TextResourceHelper.GetTextResource($"{type}Button"), CardName);

                SetData(header, message, button, success, command);

                if (type == MessagePageType.WIDNotSupported)
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_failed.svg?assembly=DigiD.Common";
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> { { "Type", type.ToString() } };
                AppCenterHelper.TrackError(ex, properties);
            }

        }

        public void SetData(string header, string message, string actionText, bool? success, ICommand command = null)
        {
            HeaderText = header;
            FooterText = message;
            ButtonText = actionText;
#if PROD
            _isSuccess = success.HasValue && success.Value;
#endif

            if (success == null)
            {
                AlternateTextImage = AppResources.AlternateTextCancelImage;
                ImageSource = "resource://DigiD.Common.Resources.digid_afbeelding_icon_geannuleerd.svg?assembly=DigiD.Common";
            }
            else if (success.Value)
            {
                if (_pageType == MessagePageType.DeactivateAppSuccess)
                {
                    AlternateTextImage = AppResources.AlternateTextSuccessGenericImage;
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_success_generic.svg?assembly=DigiD.Common";
                }
                else
                {
                    AlternateTextImage = AppResources.AlternateTextSuccessImage;
                    ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_success.svg?assembly=DigiD.Common";
                }
            }
            else
            {
                AlternateTextImage = AppResources.AlternateTextFailedImage;
                ImageSource = "resource://DigiD.Common.Resources.afbeelding_icon_failed.svg?assembly=DigiD.Common";
            }

            ButtonCommand = command ?? new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot(true);
            });
        }


#if PROD
        private bool _init;
        private bool _isSuccess;
        public override async void OnAppearing()
        {
            base.OnAppearing();

            if (_init)
                return;

            _init = true;

            if (!_isSuccess)
                return;


            var settings = DependencyService.Get<Settings.IGeneralPreferences>();
            settings.NumberOfSuccessfulLogins++;

            if (settings.NumberOfSuccessfulLogins >= Constants.AppConfigConstants.RatingLimit)
            {
                await RatingHelper.RequestRating();
                settings.NumberOfSuccessfulLogins = 0;
            }
        }
#endif
    }
}

