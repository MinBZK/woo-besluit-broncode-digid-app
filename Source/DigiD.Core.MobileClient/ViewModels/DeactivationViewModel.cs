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
﻿using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class DeactivationViewModel : BaseViewModel
    {
        public bool IsLetterActivation { get; set; }
        public string ImageSource => IsLetterActivation ? "resource://DigiD.Resources.afbeelding_activeringscode_brief.svg" : "resource://DigiD.Resources.digid_afbeelding_digid_app_deactiveren.svg";
        public DeactivationViewModel()
        {
            PageId = "AP010";
            HeaderText = AppResources.DeactivateHeader;
            HasBackButton = true;
            IsLetterActivation = DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Pending;
            FooterText = IsLetterActivation ? AppResources.DeactivateFooterLetter : AppResources.DeactivateFooter;
            
            NavCloseCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot();
            });
        }

        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () => await NavigationService.GoBack());
            }
        }

        public AsyncCommand DeactivateCommand
        {
            get
            {
                return new AsyncCommand( async () =>
                {
                    CanExecute = false;

                    try
                    {
                        if (await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.DeactivateHeader,
                            AppResources.DeactivateConfirmMessage, AppResources.ConfirmOK, AppResources.ConfirmCancel))
                        {
                            await DeactivationHelper.DeactivateApp();
                            await NavigationService.ShowMessagePage(MessagePageType.DeactivateAppSuccess);
                        }
                    }
                    catch (System.Exception e)
                    {
                        FooterText = $"Fout tijdens openen Popup scherm: {e.Message}";
                    }

                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        public override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
