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
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using System;
using System.Globalization;
using System.Windows.Input;
using DigiD.Common.EID.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.NFC.Enums;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    public class BaseConfirmViewModel : CommonBaseViewModel
    {
        public string ImageSource { get; set; }
        public string CancelText { get; set; } = AppResources.ConfirmCancel;
        protected bool IsNextCommandExecuted;

        public ConfirmModel Model { get; }

        public BaseConfirmViewModel(ConfirmModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            CancelCommand = new AsyncCommand(async () =>
            {
                if (!CanExecute)
                    return;

                CanExecute = false;
                if (Model.WIDSessionResponse != null)
                    await DependencyService.Get<IEIDServices>().CancelAuthenticate();

                Cancel();
                await NavigationService.PopToRoot(true);
                CanExecute = true;
            }, () => CanExecute);

            SetWidActions();
        }

        private void SetWidActions()
        {
            switch (Model.Action)
            {
                case AuthenticationActions.ACTIVATE_DRIVING_LICENSE:
                    {
                        ButtonCommand = ShowPINForEIDCommand;
                        PageId = Device.Idiom == TargetIdiom.Desktop ? "DA016" : "AP408";
                        HeaderText = AppResources.WIDActivateConfirmHeader;
                        FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDActivateConfirmMessage, DocumentType.DrivingLicense.Translate().ToLowerInvariant());
                        ImageSource = "resource://DigiD.Common.Resources.afbeelding_document_activeren.svg?assembly=DigiD.Common";
                        ButtonText = AppResources.ConfirmOK;

                        CardHelper.SetCard(DocumentType.DrivingLicense);
                        break;
                    }
                case AuthenticationActions.ACTIVATE_IDENTITY_CARD:
                    {
                        PageId = Device.Idiom == TargetIdiom.Desktop ? "DA016" : "AP408";
                        HeaderText = AppResources.WIDActivateConfirmHeader;
                        FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDActivateConfirmMessage, DocumentType.IDCard.Translate().ToLowerInvariant());
                        ImageSource = "resource://DigiD.Common.Resources.afbeelding_document_activeren.svg?assembly=DigiD.Common";
                        ButtonText = AppResources.ConfirmOK;
                        ButtonCommand = ShowPINForEIDCommand;
                        CardHelper.SetCard(DocumentType.IDCard);
                        break;
                    }
                case AuthenticationActions.REACTIVATE_DRIVING_LICENSE:
                case AuthenticationActions.REACTIVATE_IDENTITY_CARD:
                    {
                        var card = Model.Action == AuthenticationActions.REACTIVATE_DRIVING_LICENSE
                            ? DocumentType.DrivingLicense
                            : DocumentType.IDCard;

                        PageId = Device.Idiom == TargetIdiom.Desktop ? "DA037" : "AP413";
                        HeaderText = Device.Idiom == TargetIdiom.Desktop ? AppResources.DA037_Header : string.Format(CultureInfo.InvariantCulture, AppResources.WIDReactivateConfirmHeader, card.Translate().ToLowerInvariant());
                        FooterText = string.Format(CultureInfo.InvariantCulture, Device.Idiom == TargetIdiom.Desktop ? AppResources.DA037_Message : AppResources.WIDReactivateConfirmMessage, card.Translate().ToLowerInvariant());
                        ImageSource = "resource://DigiD.Common.Resources.afbeelding_document_vernieuwen.svg?assembly=DigiD.Common";
                        ButtonText = AppResources.ConfirmOK;
                        ButtonCommand = ShowPINForEIDCommand;
                        CardHelper.SetCard(card);
                        break;
                    }
                case ConfirmActions.ChangeWIDPIN:
                    {
                        PageId = "AP410";
                        HeaderText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDChangePINHeader, AppResources.eID.ToLowerInvariant());
                        FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDChangePINMessage, AppResources.eID.ToLowerInvariant());
                        ImageSource = "resource://DigiD.Common.Resources.digid_afbeelding_pincode_wijzigen.svg?assembly=DigiD.Common";
                        ButtonText = AppResources.Continue;
                        CancelText = string.Empty;

                        ButtonCommand = new Command(() =>
                            {
                                NavigationService.GoToNFCScannerPage(new NfcScannerModel
                                {
                                    Action = PinEntryType.ChangePIN_PIN
                                });
                            });
                        NavCloseCommand = new Command(() => NavigationService.PopToRoot());
                        break;
                    }
            }
        }

        public virtual void Cancel()
        {

        }

        public ICommand CancelCommand { get; set; }

        public AsyncCommand ShowPINForEIDCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;
                    var result = await DependencyService.Get<IEIDServices>().Confirm();
                    switch (result)
                    {
                        case ApiResult.Ok when Model.WIDSessionResponse != null:
                            {
                                var model = new NfcScannerModel
                                {
                                    IsActivation = Model.Action == AuthenticationActions.ACTIVATE_DRIVING_LICENSE || Model.Action == AuthenticationActions.ACTIVATE_IDENTITY_CARD,
                                    Action = PinEntryType.Authentication,
                                    EIDSessionResponse = Model.WIDSessionResponse,
                                };

                                await NavigationService.GoToNFCScannerPage(model);
                                break;
                            }
                        case ApiResult.Nok:
                            await NavigationService.ShowMessagePage(MessagePageType.LoginCancelled);
                            break;
                        default:
                            await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
                            break;
                    }

                    CanExecute = true;
                }, () => CanExecute);
            }
        }
    }
}
