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
using System.Windows.Input;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models;
using DigiD.Common.NFC.Enums;
using DigiD.Common.SessionModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Common.RDA.ViewModels
{
    public class AP109ViewModel : BaseViewModel
    {
#if A11YTEST
        public AP109ViewModel() : this(async (r) => { await Task.FromResult(false); }, async () => { await Task.CompletedTask; })
        {

        }
#endif
        public AP109ViewModel(Func<bool, Task> completeAction, Func<Task> retryAction)
        {
            HasBackButton = true;
            PageId = "AP109";
            ButtonCommand = new AsyncCommand(async () =>
            {
                if (string.IsNullOrEmpty(DateOfBirth) || string.IsNullOrEmpty(ExpiryDate))
                    return;

                if (!CanExecute)
                    return;

                Validate();

                if (!IsValid)
                    return;

                CanExecute = false;

                var dob = DateOfBirth.ToDate();
                var expiryDate = ExpiryDate.ToDate();

                var model = new InitForeignDocumentRequestModel
                {
                    DateOfBirth = dob.ToString("yyMMdd"),
                    DateOfExpiry = expiryDate.ToString("yyMMdd"),
                    DocumentNumber = DocumentNumber,
                    DocumentType = PassportSelected.HasValue && PassportSelected.Value ? "P" : "I"
                };

                AppSession.Process = Process.AppActivationViaMrz;
                await NavigationService.PushAsync(new AP107ViewModel(model, completeAction, retryAction));

                CanExecute = true;
            }, () => CanExecute && IsValid);

            NavCloseCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot();
            });
        }

        public bool? PassportSelected { get; set; }
        public bool? IDCardSelected { get; set; }

        public string ExpiryDate { get; set; }
        public string DateOfBirth { get; set; }

        public ICommand RadiobuttonChangedCommand => new Command<DocumentType>(d =>
        {
            PassportSelected = d == DocumentType.Passport;
            IDCardSelected = d == DocumentType.IDCard;

            Validate();
        });

        public bool IsValid { get; set; }

        public bool IsValidDob { get; set; } = true;
        public bool IsValidExpirationDate { get; set; } = true;
        public bool IsValidDocumentNumber { get; set; } = true;

        private string _documentNumber;
        public string DocumentNumber
        {
            get => _documentNumber;
            set
            {
                _documentNumber = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string SelectedDocument
        {
            get
            {
                if (PassportSelected.GetValueOrDefault(false))
                    return AppResources.Passport;
                if (IDCardSelected.GetValueOrDefault(false))
                    return AppResources.IDCard;

                return "";
            }
        }

        public Command<string> ValidateCommand => new Command<string>(Validate);

        public void Validate(string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(DocumentNumber):
                    IsValidDocumentNumber = !string.IsNullOrEmpty(DocumentNumber) && DocumentNumber.Length == 9;
                    break;
                case nameof(DateOfBirth):
                    var ds = CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.DateSeparator;
                    IsValidDob = !string.IsNullOrEmpty(DateOfBirth) && (DateOfBirth.StartsWith($"00{ds}00{ds}") || DateOfBirth == $"00{ds}00{ds}0000" || (DateTime.TryParse(DateOfBirth, out var dob) && DateTime.Now.Date > dob));
                    break;
                case nameof(ExpiryDate):
                    IsValidExpirationDate = !string.IsNullOrEmpty(ExpiryDate) && DateTime.TryParse(ExpiryDate, out var dt) && dt > DateTime.Now.Date;
                    break;
                case "All":
                    Validate(nameof(DocumentNumber));
                    OnPropertyChanged(nameof(DocumentNumber));
                    if (!IsValidDocumentNumber)
                        return;

                    Validate(nameof(DateOfBirth));
                    OnPropertyChanged(nameof(DateOfBirth));
                    if (!IsValidDob)
                        return;

                    Validate(nameof(ExpiryDate));
                    OnPropertyChanged(nameof(ExpiryDate));
                    if (!IsValidExpirationDate)
                        return;

                    break;
            }

            IsValid = !string.IsNullOrEmpty(DocumentNumber) && DocumentNumber.Length == 9 && !string.IsNullOrEmpty(DateOfBirth) && IsValidDob && !string.IsNullOrEmpty(ExpiryDate) && IsValidExpirationDate && (PassportSelected.HasValue || IDCardSelected.HasValue);
        }
    }
}
