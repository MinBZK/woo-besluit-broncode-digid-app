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
﻿using System;
using System.Threading.Tasks;
using DigiD.Common.Mobile.BaseClasses;
using Xamarin.CommunityToolkit.ObjectModel;

namespace DigiD.Common.RDA.ViewModels
{
    public class AP103ViewModel : BaseViewModel
    {
#if A11YTEST
        public AP103ViewModel() : this(async (r) => { await Task.FromResult(false); }, async () => { await Task.CompletedTask; })
        {

        }
#endif

        public AP103ViewModel(Func<bool, Task> completeAction, Func<Task> retryAction)
        {
            PageId = "AP103";
            HasBackButton = true;
            ButtonCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PushAsync(new AP109ViewModel(completeAction, retryAction));
            });
        }

        public AsyncCommand BackCommand => new AsyncCommand(async () =>
        {
            await NavigationService.GoBack();
        });
    }
}
