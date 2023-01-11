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
using System.Windows.Input;
using DigiD.Common.BaseClasses;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.SessionModels;
using FFImageLoading.Forms;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.Mobile.Controls
{
    public partial class HeaderView : ContentView
    {
        public static readonly BindableProperty MenuCommandProperty = BindableProperty.Create(nameof(MenuCommand), typeof(AsyncCommand), typeof(HeaderView));
        public static readonly BindableProperty ShowMenuProperty = BindableProperty.Create(nameof(ShowMenu), typeof(bool), typeof(HeaderView), false);
        public static readonly BindableProperty LogoutCommandProperty = BindableProperty.Create(nameof(LogoutCommand), typeof(ICommand), typeof(HeaderView), null, BindingMode.OneTime, propertyChanged: LogoutCommandPropertyChanged);

        private static void LogoutCommandPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var command = (ICommand)newvalue;
            ((HeaderView)bindable).LogoutButton.IsVisible = command != null && command.CanExecute(null);
        }

        public ICommand LogoutCommand
        {
            get => (ICommand)GetValue(LogoutCommandProperty);
            set => SetValue(LogoutCommandProperty, value);
        }

        public AsyncCommand MenuCommand
        {
            get => (AsyncCommand)GetValue(MenuCommandProperty);
            set => SetValue(MenuCommandProperty, value);
        }

        public bool ShowMenu
        {
            get => (bool)GetValue(ShowMenuProperty);
            set => SetValue(ShowMenuProperty, value);
        }

        public HeaderView()
        {
            InitializeComponent();

            SetImageSource();
            
            HandleDisplayInfoChanged(null, null);

            if (Device.RuntimePlatform == Device.Android)
            {
                AutomationProperties.SetIsInAccessibleTree(this, false);
                grid.Margin = new Thickness(0, 0, 15, 0);
            }

            AppSession.MenuItemChanged += UnreadMessagesChanged;
            DeviceDisplay.MainDisplayInfoChanged += HandleDisplayInfoChanged;
        }

        private void SetImageSource()
        {
            if (AppSession.AccountStatus?.HasUnreadMessages == true)
                MenuButtonView.SetOnAppTheme<ImageSource>(CachedImage.SourceProperty, "resource://DigiD.Resources.digid_icon_menu_nieuwe_bericht.svg", "resource://DigiD.Resources.digid_icon_menu_nieuwe_bericht_dark.svg");
            else
                MenuButtonView.SetOnAppTheme<ImageSource>(CachedImage.SourceProperty, "resource://DigiD.Resources.digid_icon_menu.svg", "resource://DigiD.Resources.digid_icon_menu_dark.svg");
        }

        private void UnreadMessagesChanged(object sender, EventArgs e)
        {
            SetImageSource();
        }

        private void HandleDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            HeaderViewControl.BackgroundColor = OrientationHelper.IsInLandscapeMode && DemoHelper.CurrentUser != null ? (Color)Application.Current.Resources["DemoBarColor"] : Color.Transparent;
            if (DemoHelper.CurrentUser != null)
            {
                demoUser1.Text = DemoHelper.CurrentUser.UserName;
                var pageId = BindingContext is CommonBaseViewModel bvm ? bvm.PageId : "AP???";
                demoUser2.Text = $"{DemoHelper.CurrentUser.UserName} - {pageId}";
            }
            HeaderViewControl.ForceLayout();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            if (sender == MenuButtonView && MenuCommand.CanExecute(null))
                await MenuCommand.ExecuteAsync();
        }

        private void LogoutButton_OnTapped(object sender, EventArgs e)
        {
            LogoutCommand?.Execute(null);
        }
    }
}
