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
﻿using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExternalLinkLabel : Label
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(ExternalLinkLabel), null, BindingMode.OneTime);
        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }

        public static readonly BindableProperty LinkTextProperty = BindableProperty.Create(nameof(LinkText), typeof(string), typeof(ExternalLinkLabel), null, BindingMode.OneTime);
        public string LinkText
        {
            get => (string)GetValue(LinkTextProperty);
            set => SetValue(LinkTextProperty, value);
        }

        public ExternalLinkLabel()
        {
            InitializeComponent();
            BindingContext = this;

            var gesture = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new AsyncCommand(async () =>
                {
                    await Launcher.OpenAsync(Uri);
                })
            };

            this.GestureRecognizers.Add(gesture);
        }
    }
}
