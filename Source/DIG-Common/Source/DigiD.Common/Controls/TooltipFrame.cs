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
﻿using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace DigiD.Common.Controls
{
    public class TooltipFrame : Frame
    {
        readonly CustomFontLabel _tooltipLabel;
        Polygon _tooltipArrow;

        string _tooltipText;
        public string TooltipText
        {
            get => _tooltipText;
            set
            {
                _tooltipText = value;
                _tooltipLabel.Text = _tooltipText;
            }
        }

        readonly int _topMargin = 5;
        Color _tooltipColor = (Color)Application.Current.Resources["TooltipColor"];
        public TooltipFrame()
        {
            CreateArrow();

            ModifyAttributes();

            _tooltipLabel = new CustomFontLabel();
            _tooltipLabel.SetDynamicResource(Label.StyleProperty, "LabelRegular");
            _tooltipLabel.VerticalOptions = LayoutOptions.StartAndExpand;
            _tooltipLabel.VerticalTextAlignment = TextAlignment.Center;
            _tooltipLabel.TextColor = (Color)Application.Current.Resources["TooltipTextColor"];

            IsVisible = false;

            Content = _tooltipLabel;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsVisible))
            {
                if (Parent is Grid grid)
                {
                    if (!grid.Children.Contains(_tooltipArrow))
                        grid.Children.Add(_tooltipArrow);

                    Grid.SetRow(_tooltipArrow, Grid.GetRow(this));
                    Grid.SetColumn(_tooltipArrow, Grid.GetColumn(this));
                }
                AutomationProperties.SetIsInAccessibleTree(_tooltipLabel, IsVisible);
                _tooltipArrow.IsVisible = IsVisible;
            }
        }

        private void ModifyAttributes()
        {
            Padding = new Thickness(10);
            if (Device.RuntimePlatform == Device.iOS)
                Margin = new Thickness(5, _topMargin + 2, 5, 0);
            else
                Margin = new Thickness(5, _topMargin * 2, 5, 0);

            BackgroundColor = _tooltipColor;
            BorderColor = _tooltipColor;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            HasShadow = false;
        }


        private void CreateArrow()
        {
            // Ondanks dat de Stroke gelijk is aan de fill is er een zwarte rand te zien
            // daarom wordt de strokeThickness 0 op iOS en wordt de fill iets anders zodat
            // het er op beide platformen goed uit ziet.
            var strokeThickness = Device.RuntimePlatform == Device.iOS ? 0 : 3;

            _tooltipArrow = new Polygon
            {
                Fill = new SolidColorBrush(_tooltipColor),
                Stroke = new SolidColorBrush(_tooltipColor),
                StrokeThickness = strokeThickness,
                IsVisible = false
            };

            // zie bovenstaande opmerking
            if (Device.RuntimePlatform == Device.iOS)
                _tooltipArrow.Points = new PointCollection { new Point(30, _topMargin * 2), new Point(34, strokeThickness), new Point(38, _topMargin * 2) };
            else
                _tooltipArrow.Points = new PointCollection { new Point(30, _topMargin * 2), new Point(33, strokeThickness), new Point(36, _topMargin * 2) };
        }
    }
}
