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
﻿using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Services;
using DigiD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AP079 : BaseContentPage
    {
        bool _isValidating;

        public double ScrollScale { get; set; } = 1.0;
        public StackOrientation HouseNrStackOrientation { get; set; } = StackOrientation.Horizontal;

        public AP079()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(100);  // anders leest ie eerst iets ander voor
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        protected async void NextButton_Clicked(System.Object sender, System.EventArgs e)
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        {
            if (_isValidating)
                return;

            _isValidating = true;

            if (BindingContext is AP079ViewModel vm)
            {
                if (vm.IsValid && vm.ButtonCommand.CanExecute(null))
                    vm.ButtonCommand.Execute(null);
                else
                {
                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                    {
                        vm.Validate("All");

                        if (string.IsNullOrEmpty(txt_bsn.Text) || !vm.IsValidBsn)
                            await InformUser(txt_bsn, txt_bsn.ErrorText);
                        else if (string.IsNullOrEmpty(txt_dob.Text) || !vm.IsValidDob)
                            await InformUser(txt_dob, txt_dob.ErrorText);
                        else if (string.IsNullOrEmpty(txt_zipcode.Text) || !vm.IsValidPostalcode)
                            await InformUser(txt_zipcode, txt_zipcode.ErrorText);
                        else if (string.IsNullOrEmpty(txt_housenumber.Text) || !vm.IsValidHouseNumber)
                            await InformUser(txt_housenumber, $"{AppResources.InvalidInput}, {txt_housenumber.LabelText}");
                    }
                }
            }
            
            _isValidating = false;
        }

        private static async Task InformUser(ICustomEntry entry, string errorText, bool longDelay = false)
        {
            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
            {
                await Task.Delay(100);
                entry.Focus();
                // vertraging voldoende voor uitspreken bij invalid geb.datum inclusief invoer vd gebruiker
                await DependencyService.Get<IA11YService>().Speak(errorText, longDelay ? 6000 : 3000);
            }
        }

        protected async void txt_Completed(object sender, System.EventArgs e)
        {
            if (sender is ICustomEntry cef && BindingContext is AP079ViewModel vm)
            {
                var name = cef.AutomationId;
                vm.Validate(name);
                if (!cef.IsValid && DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                    await InformUser(cef, cef.ErrorText);
            }
        }
    }
}
