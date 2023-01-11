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
