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
﻿using DigiD.Common.Constants;
using DigiD.Common.Enums;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public class PinViewTiles : ContentView
    {
        public static readonly BindableProperty NumberOfTilesProperty = BindableProperty.Create(nameof(NumberOfTiles), typeof(int), typeof(PinViewTiles), 0, propertyChanged: RenderTiles);
        public static readonly BindableProperty ActiveTileIndexProperty = BindableProperty.Create(nameof(ActiveTileIndex), typeof(int), typeof(PinViewTiles), 0, propertyChanged: ActiveTileIndexPropertyChanged);
        public static readonly BindableProperty HidePinsterProperty = BindableProperty.Create(nameof(HidePinster), typeof(bool), typeof(PinViewTiles), false, propertyChanged: RenderTiles);

        private static void RenderTiles(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (PinViewTiles)bindable;
            view.RenderTiles();
        }

        private static void ActiveTileIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var next = (int)oldValue < (int)newValue;
            var index = (int)newValue;

            var control = (PinViewTiles)bindable;
            var grid = (Grid)control.Content;

            if (index == 0)
            {
                control.RenderTiles();
                return;
            }

            if (control.NumberOfTiles > index)
            {
                var currentTile = (PinTile)grid.Children[index];
                currentTile.State = PinTileState.Current;
            }

            if (next)
            {
                var previousTile = (PinTile)grid.Children[index - 1];
                previousTile.State = PinTileState.Active;
            }
            else
            {
                var previousTile = (PinTile)grid.Children[index + 1];
                previousTile.State = PinTileState.Inactive;
            }
        }

        public int NumberOfTiles
        {
            get => (int)GetValue(NumberOfTilesProperty);
            set => SetValue(NumberOfTilesProperty, value);
        }

        public int ActiveTileIndex
        {
            get => (int)GetValue(ActiveTileIndexProperty);
            set => SetValue(ActiveTileIndexProperty, value);
        }

        public bool HidePinster
        {
            get => (bool)GetValue(HidePinsterProperty);
            set => SetValue(HidePinsterProperty, value);
        }

        public void Error()
        {
            Xamarin.Essentials.Vibration.Vibrate(500);
            MessagingCenter.Send(this, MessagingConstants.PinError);
        }

        public void RenderTiles()
        {
            var grid = new Grid();

            if (Device.Idiom != TargetIdiom.Phone)
                grid.HorizontalOptions = LayoutOptions.Center;

            var width = GridLength.Star;

            for (var x = 0; x <= NumberOfTiles - 1; x++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
                grid.Children.Add(new PinTile(NumberOfTiles, x, HidePinster), x, 0);               
            }

            Content = grid;
        }
    }
}
