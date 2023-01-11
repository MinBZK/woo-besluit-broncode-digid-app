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
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Services;
using System;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.SessionModels;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class BasePinCodeViewModel : CommonBaseViewModel
    {
        protected SecureString Pin1 { get; set; } = new SecureString();
        protected SecureString Pin2 { get; set; } = new SecureString();
        protected SecureString Pin3 { get; set; } = new SecureString();

        public abstract Task PINCompleteAction();
        public abstract Task ChangePINAction();

        private readonly DelegateWeakEventManager _pinErrorWeakEventManager = new DelegateWeakEventManager();
        public event EventHandler PinErrorEvent
        {
            add => _pinErrorWeakEventManager.AddEventHandler(value);
            remove => _pinErrorWeakEventManager.RemoveEventHandler(value);
        }

        public int NumberOfTiles { get; set; } = 10;
        public bool IsCANInput { get; set; }
        public string HelpText { get; set; }

        public InfoPageType InfoPageType { get; set; }
        protected PinCodeModel Model { get; }

        public int ActivePinTile { get; set; }
        public bool IsPIN1Active { get; set; } = true;
        public bool IsPIN2Active { get; set; }

        private bool _init;
        private readonly bool _fromInit = true;
        private readonly string _documentName;

        protected BasePinCodeViewModel(PinCodeModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            if (EIDSession.Card != null)
                _documentName = EIDSession.Card.DocumentType.Translate().ToLowerInvariant();

            switch (Model.EntryType)
            {
                case PinEntryType.Authentication:

                    PageId = Device.Idiom == TargetIdiom.Desktop ? "DA014" : "AP016";

                    if (model.EIDSessionResponse == null)
                    {
                        InfoPageType = AppSession.IsAppActivated ? InfoPageType.PinActivated : InfoPageType.PinRegistration;

                        var message = AppResources.AuthenticationMessage;
                        if (model.IsAppAuthentication)
                            message = AppResources.AppAuthenticationMessage;
                        else if (model.ChangeAppPin)
                            message = AppResources.ChangeAppPinMessage;

                        SetLabels(AppSession.Process == Process.UpgradeAndAuthenticate ? AppResources.LoginAndUpgradeHeader : AppResources.AuthenticationHeader, message);
                    }
                    else
                    {
                        InfoPageType = InfoPageType.WIDHelpEnterPIN;
                        SetLabels(AppResources.AuthenticationHeader, EIDSession.Card != null ? string.Format(CultureInfo.InvariantCulture, AppResources.DA014_Message, _documentName) : AppResources.AuthenticationMessage);
                    }

                    SetNumberOfTiles(PincodeType.PIN);
                    break;
                case PinEntryType.Enrollment:
                    PageId = "AP006";
                    SetLabels(AppResources.EnterPinHeader, AppResources.EnterPinMessage);
                    SetNumberOfTiles(PincodeType.PIN);
                    InfoPageType = InfoPageType.PinRegistration;
                    break;
                case PinEntryType.ChangePIN_PIN:
                    SetNumberOfTiles(PincodeType.PIN);
                    InfoPageType = InfoPageType.WIDHelpChangePIN_PIN;
                    PageId = Device.Idiom == TargetIdiom.Desktop ? "DA009" : "AP420";
                    SetLabels(string.Format(AppResources.WIDChangePIN_PINHeader, _documentName), string.Format(CultureInfo.InvariantCulture, AppResources.WIDChangePIN_PINMessage, _documentName));
                    break;
                case PinEntryType.ChangeTransportPIN:
                    InfoPageType = InfoPageType.WIDHelpChangeTransportPIN;
                    PageId = Device.Idiom == TargetIdiom.Desktop ? "DA018" : "AP401";
                    HelpText = AppResources.DA018_HelpText;
                    Pin1 = model.PIN;
                    IsPIN1Active = false;
                    IsPIN2Active = true;
                    SetNumberOfTiles(PincodeType.PIN);
                    SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDNewPINHeader, _documentName), string.Format(CultureInfo.InvariantCulture, AppResources.WIDNewPINMessage, _documentName));
                    break;
                case PinEntryType.ResumePIN_CAN:
                    InfoPageType = InfoPageType.WIDHelpResumePIN;
                    PageId = Device.Idiom == TargetIdiom.Desktop ? "DA114" : "AP418";
                    SetNumberOfTiles(PincodeType.CAN);
                    SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDResumePIN_CANHeader, model.Card.CANName.FirstCharToUpper()), string.Format(CultureInfo.InvariantCulture, AppResources.WIDResumePIN_CANMessage, model.Card.CANName));
                    break;
            }

            if (Model.TriesLeft != null && (Model.EntryType == PinEntryType.ChangePIN_PIN || Model.EntryType == PinEntryType.Authentication))
            {
                if (Model.NeedPinChange && Device.Idiom == TargetIdiom.Desktop)
                {
                    PageId = "DA117";
                    SetLabels(AppResources.DA117_Header, string.Format(CultureInfo.InvariantCulture, AppResources.DA117_Message, Model.TriesLeft.Value, Model.TriesLeft.Value > 1 ? AppResources.PluralSuffix : string.Empty), true);
                }
                else
                {
                    PageId = Device.Idiom == TargetIdiom.Desktop ? "DA113" : "AP231";
                    SetLabels(HeaderText, string.Format(CultureInfo.InvariantCulture, AppResources.WIDWrongPINMessage, "pin", Model.TriesLeft.Value, Model.TriesLeft.Value > 1 ? AppResources.PluralSuffix : string.Empty), true);
                }
            }
            else if (Model.WrongCAN)
            {
                PageId = Device.Idiom == TargetIdiom.Desktop ? "DA115" : "AP242";
                SetLabels(HeaderText, string.Format(CultureInfo.InvariantCulture, AppResources.WIDWrongCANMessage, Model.Card.CANName), true);
            }

            _fromInit = false;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (_init)
                return;

            ActivePinTile = 0;
            Pin2 = new SecureString();
            Pin3 = new SecureString();

            if (Model.EntryType != PinEntryType.ChangeTransportPIN)
            {
                IsPIN1Active = true;
                IsPIN2Active = false;

                Pin1 = new SecureString();
            }

            _init = true;
        }

        public void SetNumberOfTiles(PincodeType pincodeType)
        {
            IsCANInput = pincodeType == PincodeType.CAN;

            switch (pincodeType)
            {
                case PincodeType.PIN:
                    NumberOfTiles = 5;
                    break;
                case PincodeType.CAN:
                    NumberOfTiles = Model.Card.CANLength;
                    break;
                case PincodeType.PUK:
                    NumberOfTiles = 8;
                    break;
            }

            for (var x = 0; x <= NumberOfTiles - 1;x++)
                SetPinTileText(x, string.Empty);
        }

        public void SetLabels(string header, string message, bool isError = false)
        {
            IsError = isError;
            HeaderText = header;

            var vo = DependencyService.Get<IA11YService>().IsInVoiceOverMode();

            if (!_fromInit && vo)
            {
                FooterText = "";    // Forceer een LiveUpdate
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(Device.RuntimePlatform == Device.iOS ? 500 : 10);
                    FooterText = message;
                });
            }
            else
                FooterText = message;

            AccessibilityFooterText = "";    // Forceer een LiveUpdate
            AccessibilityFooterText = message.ChangeForA11y();
        }

        public AsyncCommand<int> PinNumberEnteredCommand
        {
            get
            {
                return new AsyncCommand<int>(async number =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;

                    if (ActivePinTile < NumberOfTiles)
                    {
                        if (number >= 0 && number <= 9)
                        {
                            if (IsPIN1Active)
                                Pin1.AppendChar(number);
                            else if (IsPIN2Active)
                                Pin2.AppendChar(number);
                            else
                                Pin3.AppendChar(number);

                            if (IsCANInput)
                                SetPinTileText(ActivePinTile, number.ToString());

                            ActivePinTile++;

                            if (ActivePinTile < NumberOfTiles)
                                await DependencyService.Get<IA11YService>().Speak($"{ActivePinTile} {AppResources.AccessibilityOf} {NumberOfTiles}", 500);

                            if (Model.EntryType == PinEntryType.Enrollment && ActivePinTile == NumberOfTiles && IsPIN1Active)
                            {
                                await Task.Delay(200);
                                var result = PinCodeHelper.IsPinCodeComplexEnough(Pin1);
                                //Check for if entered pincode meets complexity requirements
                                if (result.Success)
                                {
                                    SetLabels(AppResources.ReEnterPinHeader, AppResources.ReEnterPinMessage);
                                    IsPIN1Active = false;
                                    IsPIN2Active = true;
                                }
                                else
                                {
                                    OnError();
                                    Pin1 = new SecureString();
                                    IsPIN1Active = true;
                                    IsPIN2Active = false;
                                    SetLabels(result.Header, result.Message, true);
                                }

                                ActivePinTile = 0;
                            }
                            else if (Model.EntryType != PinEntryType.Authentication && ActivePinTile == NumberOfTiles)
                            {
                                await Task.Delay(200);

                                if (IsPIN1Active)
                                {
                                    SetNumberOfTiles(PincodeType.PIN);

                                    if (Model.EntryType == PinEntryType.ResumePIN_CAN)
                                    {
                                        if (Model.EIDSessionResponse != null)//Indien kaart is gesuspend in een authenticatie sessie, oude PIN vragen
                                        {
                                            SetLabels(AppResources.AuthenticationHeader, AppResources.AuthenticationMessage);
                                        }
                                        else
                                        {
                                            SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDExistingPINHeader, _documentName), string.Format(CultureInfo.InvariantCulture, AppResources.WIDExistingPINMessage, _documentName));
                                            PiwikHelper.TrackView("AP400", GetType().FullName);

                                            if (Model.TriesLeft.HasValue)
                                                FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDWrongPIN_CANMessage, "pin", Model.TriesLeft.Value, Model.TriesLeft.Value > 1 ? "en" : "");

                                            SetNumberOfTiles(Model.EntryType == PinEntryType.ResumePIN_CAN ? PincodeType.PIN : PincodeType.PUK);
                                        }
                                    }
                                    else
                                    {
                                        SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDNewPINHeader, _documentName), string.Format(CultureInfo.InvariantCulture, AppResources.WIDNewPINMessage, _documentName));
                                        PageId = "DA010";
                                        HelpText = AppResources.DA010_HelpText;
                                        TrackView();
                                    }

                                    ActivePinTile = 0;
                                    IsPIN1Active = false;
                                    IsPIN2Active = true;
                                }
                                else if (IsPIN2Active && Model.EntryType != PinEntryType.Enrollment)
                                {
                                    if (Model.EntryType == PinEntryType.ResumePIN_CAN)
                                        await ShowNFCScanner();
                                    else
                                    {
                                        var result = PinCodeHelper.IsPinCodeComplexEnough(Pin2);

                                        if (result.Success)
                                        {
                                            if (Model.ChangeAppPin)
                                            {
                                                SetLabels(AppResources.AppChangePinHeader, AppResources.AppChangePinReenterMessage);
                                                PiwikHelper.TrackView("AP002", GetType().FullName);
                                            }
                                            else
                                                SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDReenterPINHeader, _documentName), string.Format(CultureInfo.InvariantCulture, AppResources.WIDReenterPINMessage, _documentName));
                                            
                                            ActivePinTile = 0;
                                            IsPIN2Active = false;
                                            PiwikHelper.TrackView("AP402", GetType().FullName);

                                        }
                                        else
                                        {
                                            OnError();
                                            Pin2 = new SecureString();
                                            SetLabels(result.Header, result.Message, true);
                                            ActivePinTile = 0;
                                        }
                                    }
                                }
                                else if (Model.EntryType != PinEntryType.Enrollment)
                                {
                                    if (Pin2.ToPlain() == Pin3.ToPlain())
                                    {
                                        if (Model.ChangeAppPin)
                                            await ChangePINAction();
                                        else
                                            await ShowNFCScanner();
                                    }
                                    else
                                    {
                                        OnError();
                                        Pin2 = new SecureString();
                                        Pin3 = new SecureString();
                                        IsPIN2Active = true;
                                        SetLabels(string.Format(CultureInfo.InvariantCulture, AppResources.WIDNewPINHeader, _documentName), Device.Idiom == TargetIdiom.Desktop ? AppResources.DA112_Message : AppResources.PincodeMatchErrorMessage, true);
                                        ActivePinTile = 0;
                                    }
                                }
                            }
                        }
                        else if (number == 11 && ActivePinTile > 0)
                        {
                            //backspace clicked
                            SetPinTileText(ActivePinTile - 1, string.Empty);
                            ActivePinTile--;

                            if (IsPIN1Active)
                                Pin1.RemoveChar();
                            else if (IsPIN2Active)
                                Pin2.RemoveChar();
                            else
                                Pin3.RemoveChar();
                        }

                        if (ActivePinTile == NumberOfTiles && (Model.EntryType == PinEntryType.Authentication || Model.EntryType == PinEntryType.Enrollment))
                        {
                            await Task.Delay(200);
                            //Pin complete so start validation!
                            await PINCompleteAction();
                        }
                    }

                    CanExecute = true;
                }, p => CanExecute);
            }
        }

        public void OnError()
        {
            _pinErrorWeakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(PinErrorEvent));
        }

        public async Task ShowNFCScanner()
        {
            var model = new NfcScannerModel
            {
                Action = Model.EntryType,
                PIN = Pin1,
                EIDSessionResponse = Model.EIDSessionResponse,
                NewPIN = Pin2,
                IsStatusChecked = Model.IsStatusChecked,
                Card = Model.Card,
                IsActivation = Model.IsWIDActivation
            };

            await NavigationService.GoToNFCScannerPage(model);
        }

        #region Tiles

        public string Pin1Text { get; set; }
        public string Pin2Text { get; set; }
        public string Pin3Text { get; set; }
        public string Pin4Text { get; set; }
        public string Pin5Text { get; set; }
        public string Pin6Text { get; set; }
        public string Pin7Text { get; set; }
        public string Pin8Text { get; set; }
        public string Pin9Text { get; set; }
        public string Pin10Text { get; set; }

        public void SetPinTileText(int index, string text)
        {
            var pi = GetType().GetProperty($"Pin{index + 1}Text");
            pi?.SetValue(this, Convert.ChangeType(text, pi.PropertyType, CultureInfo.InvariantCulture), null);
        }

        #endregion
    }
}
