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
using DigiD.Common.Models;
using DigiD.Common.NFC.Enums;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DigiD.Common.ViewModels
{
    public class WidSuspendedViewModel : CommonBaseViewModel
    {
        public string AnimationSource { get; set; }

        public WidSuspendedViewModel(PinCodeModel model)
        {
            HeaderText = AppResources.WidSuspended_Header;

            switch (model.Card.DocumentType)
            {
                case DocumentType.DrivingLicense:
                    PageId = "AP404";
                    FooterText = AppResources.AP404_Message;
                    AnimationSource = "animatie_CAN_code_uitleg_rijbewijs.json";
                    break;
                case DocumentType.IDCard:
                    PageId = "AP406";
                    AnimationSource = "animatie_CAN_code_uitleg_IDkaart.json";
                    FooterText = AppResources.AP406_Message;
                    break;
            }
            
            ButtonCommand = new AsyncCommand(async () =>
            {
                await NavigationService.GoToPincodePage(model);
            });
        }
    }
}
