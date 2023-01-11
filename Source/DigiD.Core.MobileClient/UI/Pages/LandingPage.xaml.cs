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
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Controls;
using DigiD.UI.Popups;
using DigiD.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Application = Xamarin.Forms.Application;

namespace DigiD.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPage : BaseContentPage
    {
        public MainMenuPopup MenuPopup { get; private set; }
        public MainMenuPage MenuPage { get; private set; }

        public LandingPage()
        {
            InitializeComponent();

            if (NavigationPage.GetTitleView(this) is HeaderView headerView)
            {
                headerView.MenuCommand = new AsyncCommand(async () =>
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        MenuPage ??= new MainMenuPage();
                        await Application.Current.MainPage.Navigation.PushModalAsync(MenuPage, false);
                    }
                    else
                    {
                        MenuPopup ??= new MainMenuPopup();
                        await Application.Current.MainPage.Navigation.ShowPopupAsync(MenuPopup);
                    }
                });
            }
        }

        public async Task CloseMenu()
        {
            if (Device.RuntimePlatform == Device.Android)
                MenuPopup?.Close();
            else if (Navigation.ModalStack.Count > 0 && MenuPage != null)
                await MenuPage.CloseMenu();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
#if !PROD
            if (letterImage.GestureRecognizers.Count == 0)
            {
                letterImage.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = ((LandingViewModel)BindingContext).ChangeLetterRequestDate,
                    NumberOfTapsRequired = 4
                });
            }
#endif

            if (NavigationPage.GetTitleView(this) is HeaderView headerView)
            {
                headerView.LogoutCommand = ((LandingViewModel)BindingContext).LogoutCommand;
            }
        }
    }
}
