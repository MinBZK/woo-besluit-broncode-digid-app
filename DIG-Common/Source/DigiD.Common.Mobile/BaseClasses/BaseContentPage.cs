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
using System.Collections.ObjectModel;
using System.Linq;
using DigiD.Common.BaseClasses;
using DigiD.Common.Controls;
using DigiD.Common.Mobile.Controls;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Application = Xamarin.Forms.Application;

namespace DigiD.Common.Mobile.BaseClasses
{
    public class BaseContentPage : CommonBaseContentPage
    {
        /// <summary>
        /// Onderstaande property wordt gebruikt om voor accessibility de focus op
        /// eerste (default element) te zetten, zodat de blinde gebruiker direct bij
        /// de juiste actie is. De CustomContentPageRenderer bevat een functie, FindDefaultElement()
        /// die gebruikt onderstaande property om de Accessibility "Cursor" goed te zetten.
        /// </summary>
        private CustomFontLabel _headerLabel;
        private View _headerBar;
        
        private bool _showMenu;
        public bool ShowMenu
        {
            get => _showMenu;
            set
            {
                _showMenu = value;
                if (value)
                    HasNavigationView = true;
            }
        }

        private bool _hasNavigationView;
        public bool HasNavigationView
        {
            get => _hasNavigationView;
            set
            {
                _hasNavigationView = value;
                SetTitleView();
            }
        }

        public bool ScrollHeaderEnabled { get; set; }

        public BaseContentPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersHomeIndicatorAutoHidden(true);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.None);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            Xamarin.Forms.NavigationPage.SetHasBackButton(this, true);
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, string.Empty);
        }

        private void SetTitleView()
        {
            var titleView = new HeaderView
            {
                ShowMenu = ShowMenu
            };

            Xamarin.Forms.NavigationPage.SetTitleView(this, HasNavigationView ? titleView : null);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, HasNavigationView || ScrollHeaderEnabled);
        }

        private static CustomFontLabel FindHeaderLabel(ReadOnlyCollection<Element> elements)
        {
            foreach (var element in elements)
            {
                if (element.LogicalChildren.Count > 0)
                {
                    var e = FindHeaderLabel(element.LogicalChildren);
                    if (e != null)
                        return e;
                }

                if (element is CustomFontLabel lbl && lbl.Style?.BaseResourceKey == "TitleStyle")
                    return lbl;
            }

            return null;
        }

        protected void ScrollView_OnScrolled(object sender, ScrolledEventArgs e)
        {
            var headerHeight = _headerBar.Height;
            var labelHeight = _headerLabel.Height;
            var factor = headerHeight / labelHeight;

            var y = headerHeight - e.ScrollY * factor;

            if (y <= 0)
                y = 0;

            AutomationProperties.SetIsInAccessibleTree(_headerBar, y == 0);

            _headerBar.TranslationY = y;

            if (y > 0)
                SetTitleView();
            else
            {
                Xamarin.Forms.NavigationPage.SetTitleView(this, _headerBar);
                AutomationProperties.SetIsInAccessibleTree(_headerBar, false);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

#if A11YTEST
            Title = BindingContext.GetType().Name;
#endif
            _headerLabel = FindHeaderLabel(this.Content.LogicalChildren);

            if (BindingContext is CommonBaseViewModel bvm)
            {
                Xamarin.Forms.NavigationPage.SetHasBackButton(this, bvm.HasBackButton);
                if (bvm.NavCloseButtonVisible)
                    SetNavCloseButton(bvm);
            }

            if (ScrollHeaderEnabled && _headerLabel != null)
            {
                var lbl = new CustomFontLabel
                {
                    Text = _headerLabel.Text,
                    Style = (Style)Application.Current.Resources["LabelNavbarHeader"],
                    TranslationY = 50
                };
                Xamarin.Forms.NavigationPage.SetTitleView(this, lbl);

                _headerBar = Xamarin.Forms.NavigationPage.GetTitleView(this);
                AutomationProperties.SetIsInAccessibleTree(_headerBar, false);
            }
        }

        protected void SetNavCloseButton(CommonBaseViewModel bvm)
        {
            // Right ToolbartItem X, visible maken.
            var items = ToolbarItems;

            if (items.Count == 0)
            {
                if (bvm == null)
                    return;

                var toolbarItem = new ToolbarItem
                {
                    Command = bvm.NavCloseCommand,
                    IsEnabled = true
                };
                SetCloseIcon(toolbarItem);

                AutomationProperties.SetName(toolbarItem, AppResources.AccessibilityToolbarItemCancelHelpText);
                items.Add(toolbarItem);
            }
            else
            {
                var item = ToolbarItems.FirstOrDefault(i => i.IconImageSource != null &&
                    i.IconImageSource.ToString().StartsWith("icon_sluiten", StringComparison.InvariantCultureIgnoreCase));

                if (item != null)
                {
                    SetCloseIcon(item);
                }
            }
        }

        private static void SetCloseIcon(BindableObject toolbarItem)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!Xamarin.Essentials.AppInfo.PackageName.Contains(".digid.id-checker."))
                {
                    toolbarItem.SetOnAppTheme<FileImageSource>(MenuItem.IconImageSourceProperty, "icon_sluiten.png", "icon_sluiten_dark.png");
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is CommonBaseViewModel bc)
            {
                Xamarin.Forms.NavigationPage.SetHasBackButton(this, bc.HasBackButton);
            }
        }
    }
}
