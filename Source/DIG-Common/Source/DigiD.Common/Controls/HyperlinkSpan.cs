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
using DigiD.Common.Helpers;
using DigiD.Common.SessionModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Browser = Xamarin.Essentials.Browser;

namespace DigiD.Common.Controls
{
    public class HyperlinkSpan : Span
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(string), typeof(HyperlinkSpan));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public HyperlinkSpan()
        {
            TextDecorations = TextDecorations.Underline;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Xamarin.Essentials.
                Command = new AsyncCommand(async  () =>
                {
                    if (AppSession.IsAppActivated && Source.IsUrlLinkToMyDigid())
                        await Browser.OpenAsync(AppSession.MyDigiDUrl, BrowserLaunchMode.External);
                    else
                        await Launcher.OpenAsync(new Uri(Source));
                }),
                NumberOfTapsRequired = 1
            });
        }
    }
}
