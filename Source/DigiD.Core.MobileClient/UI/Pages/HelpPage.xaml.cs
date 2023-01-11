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
using System.Threading.Tasks;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using DigiD.Common.ViewModels;
using LabelHtml.Forms.Plugin.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Application = Xamarin.Forms.Application;

namespace DigiD.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        public HelpPage(InfoViewModel model)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            BindingContext = model;
            InitializeComponent();
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
        }

        private async void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            ClosePopup();
            await Task.Delay(10);
            await DependencyService.Get<IPopupService>().OpenPopup(BindingContext as InfoViewModel);
        }

        private void ClosePopup()
        {
            DeviceDisplay.MainDisplayInfoChanged -= DeviceDisplay_MainDisplayInfoChanged;
            Application.Current.MainPage.Navigation.PopModalAsync(false);
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 26 to the 15 allowed.
        protected override void OnBindingContextChanged()
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 26 to the 15 allowed.
        {
            base.OnBindingContextChanged();
            if (Device.RuntimePlatform == Device.Android && DependencyService.Get<IA11YService>().IsInVoiceOverMode() && BindingContext is InfoViewModel infoVM && infoVM.Message.Contains("<ul>"))
            {
                var afterList = infoVM.Message.Split("</ul>")[1];
                var beforeList = infoVM.Message.Split("<ul>")[0];
                var listItSelf = infoVM.Message.Split("<ul>")[1].Split("</ul>")[0];

                var items = listItSelf.Split("</li>");
                var endUlTagAdded = false;

                var index = container.Children.IndexOf(messageHtmlLabel);
                container.Children.Insert(++index, GetLabel(beforeList, endTag: "<ul>"));
                for (int x = 0; x < items.Length; x++)
                {
                    var item = items[x];
                    if (!string.IsNullOrEmpty(item))
                        container.Children.Insert(++index, GetLabel(item, endTag: "</li>"));

                    if (x > 0 && string.IsNullOrEmpty(item))
                    {
#pragma warning disable S1643 //Code Smell: Use a StringBuilder instead.
                        ((HtmlLabel)container.Children[x - 1]).Text += "</ul>";
#pragma warning restore S1643 //Code Smell: Use a StringBuilder instead.
                        endUlTagAdded = true;
                    }
                }
                container.Children.Insert(++index, GetLabel(afterList, endUlTagAdded ? "" : "</ul>"));

                messageHtmlLabel.IsVisible = false; //oorspronkelijke tekst verbergen
            }
        }

        private static HtmlLabel GetLabel(string item, string startTag = "", string endTag = "")
        {
            var newText = startTag + item + endTag;

            return new HtmlLabel
            {
                Margin = new Thickness(0),
                Style = (Style)Application.Current.Resources["HtmlLabelRegular"],
                Text = newText
            };
        }

        private void ReadButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
        }

        private void Layout_SizeChanged(object sender, EventArgs e)
        {
            scrollView.WidthRequest = Math.Min(App.PopupWidth, container.Width);
            scrollView.HeightRequest = Math.Min(App.PopupHeight, container.Height);
        }
    }
}
