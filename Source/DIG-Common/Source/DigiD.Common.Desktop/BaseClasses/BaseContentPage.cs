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
﻿using DigiD.Common.Services;
using Xamarin.Forms;

namespace DigiD.Common.BaseClasses
{
    public abstract class BaseContentPage : CommonBaseContentPage
    {
        protected BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            SetDynamicResource(BackgroundColorProperty, "PageBackgroundColor");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                DependencyService.Get<IA11YService>().Speak(((BaseViewModel)BindingContext).PageIntroName);
        }
    }
}
