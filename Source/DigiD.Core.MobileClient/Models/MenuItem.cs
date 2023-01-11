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
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.BaseClasses;
using DigiD.Common.Helpers;
using DigiD.Helpers;
using DigiD.UI.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Models
{
    public class MenuItem : BindableObject
    {
        public static BindableProperty IconSourceProperty = BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(MenuItem));

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                AccessibilityText = value;
            }
        }

        public Thickness IconMargin { get; set; } = new Thickness(10);

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        private string _icon;
        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                this.SetOnAppTheme<ImageSource>(IconSourceProperty, Icon, ThemeHelper.Convert(Icon, true));
            }
        }

        public bool IsChevronVisible => TargetViewModel != null || !IsExternalLink;
        public Type TargetViewModel { get; set; }
        public object[] Arguments { get; set; }
        public Func<Task> Action { get; set; }

        // KTR: voorkomt van het onterecht niet tonen van chevron en foutief
        // voorlezen tekst in A11Y-mode.
        public bool IsExternalLink { get; set; } = false;

        private string _accessibilityText;
        public string AccessibilityText
        {
            get
            {
                if (string.IsNullOrEmpty(_accessibilityText))
                {
                    _accessibilityText = Title;
                }
                return _accessibilityText + (IsChevronVisible ? string.Empty : $", {AppResources.AccessibilityMenuItemLinkText}");
            }
            set => _accessibilityText = value;
        }

        public AsyncCommand ItemSelectedCommand => new AsyncCommand(async () =>
        {
            if (Application.Current.MainPage is CustomNavigationPage { RootPage: LandingPage lp })
                await lp.CloseMenu();

            if (TargetViewModel != null)
                await NavigationHelper.PushTargetViewModel(TargetViewModel, Arguments);
            else
                Action?.Invoke();
        });
    }

    public class MenuBlock
    {
        public string Header { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public MenuBlock(string header, List<MenuItem> menuItems)
        {
            Header = header;
            MenuItems = menuItems;
        }
    }
}
