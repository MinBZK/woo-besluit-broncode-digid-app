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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DigiD.Common.Enums;
using DigiD.Common.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinTilesCodeEntryView : FlexLayout
    {
        public static readonly BindableProperty InputLengthProperty = BindableProperty.Create(nameof(InputLength), typeof(int), typeof(PinTilesCodeEntryView), 0, BindingMode.OneTime, propertyChanged: InputLengthPropertyChanged);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(PinTilesCodeEntryView), "", BindingMode.TwoWay, propertyChanged: TextPropertyChanged);
        public static readonly BindableProperty ChunkSizeProperty = BindableProperty.Create(nameof(ChunkSize), typeof(int), typeof(PinTilesCodeEntryView), 0, BindingMode.TwoWay, propertyChanged: ChunkSizePropertyChanged);
        public static readonly BindableProperty ChunkOrientationProperty = BindableProperty.Create(nameof(ChunkOrientation), typeof(ChunkOrientationEnum), typeof(PinTilesCodeEntryView), ChunkOrientationEnum.Horizontal, BindingMode.TwoWay, propertyChanged: ChunkOrientationPropertyChanged);

        private static void ChunkSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PinTilesCodeEntryView entryView)
            {
                entryView.DrawContent();
            }
        }

        private static void ChunkOrientationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PinTilesCodeEntryView entryView)
            {
                entryView.DrawContent();
            }
        }

        private static void InputLengthPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is PinTilesCodeEntryView entryView)
            {
                entryView.txt.MaxLength = (int)newvalue;
                entryView.DrawContent();
                entryView.SetPlaceholder();
            }
        }

        private static void TextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is PinTilesCodeEntryView entryView)
            {
                entryView.SetPlaceholder();
            }
        }

        public int InputLength
        {
            get => (int)GetValue(InputLengthProperty);
            set => SetValue(InputLengthProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int ChunkSize
        {
            get => (int)GetValue(ChunkSizeProperty);
            set => SetValue(ChunkSizeProperty, value);
        }
        public ChunkOrientationEnum ChunkOrientation
        {
            get => (ChunkOrientationEnum)GetValue(ChunkOrientationProperty);
            set => SetValue(ChunkOrientationProperty, value);
        }

        public int ColumnSpacing { get; set; } = 10;

        protected readonly Grid _grid = new Grid();
        protected readonly bool _isPhone;

        private int _numberOfSeparators = 0;

        public PinTilesCodeEntryView()
        {
            _isPhone = Device.Idiom == TargetIdiom.Phone;

            InitializeComponent();

            _grid.HorizontalOptions = _isPhone ? LayoutOptions.CenterAndExpand : LayoutOptions.Center;
            _grid.ColumnSpacing = ColumnSpacing;
            Children.Add(_grid);

            DrawContent();
            txt.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeCharacter);

            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
            {
                entryView.JustifyContent = FlexJustify.Start;
                this.SetAppThemeColor(BackgroundColorProperty, (Color)Application.Current.Resources["Gray6"], (Color)Application.Current.Resources["Gray7"]);
                SetTextEntryForA11y();
                _grid.IsVisible = false;
            }

            txt.FontFamily = "RO-Regular";
        }

        private void SetTextEntryForA11y()
        {
            SetGrow(txt, 1.0f);
            txt.MaxLength = InputLength;

            txt.SetDynamicResource(Entry.TextColorProperty, "PageBackgroundColor");
            txt.SetDynamicResource(Entry.PlaceholderColorProperty, "PageBackgroundColor");
            txt.Margin = new Thickness(1);
            txt.IsVisible = true;

            AutomationProperties.SetIsInAccessibleTree(this, false);
            AutomationProperties.SetIsInAccessibleTree(txt, true);
            SetPlaceholder();

            txt.BackgroundColor = (Color)Application.Current.Resources["CustomEntryFieldBackgroundColor"];
            txt.PlaceholderColor = txt.BackgroundColor;

            txt.TextColor = (Color)Application.Current.Resources["CustomEntryFieldEntryTextColor"];
            txt.FontFamily = "RO-Regular";
            txt.FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Entry));

            txt.HeightRequest = 48;
            txt.HorizontalOptions = LayoutOptions.StartAndExpand;
            txt.MinimumHeightRequest = 48;
            txt.WidthRequest = txt.MaxLength * txt.FontSize;

            if (Device.RuntimePlatform == Device.Android)
            {
                txt.VerticalOptions = LayoutOptions.EndAndExpand;
                txt.VerticalTextAlignment = TextAlignment.End;
                txt.HorizontalTextAlignment = TextAlignment.Start;
            }
        }

        private void SetPlaceholder()
        {
            var name = AutomationProperties.GetName(this) ?? "";
            var suffix = $", {txt.MaxLength} {(txt.Keyboard == Keyboard.Numeric ? AppResources.AccessibilityMaxNumberOfNumbers : AppResources.AccessibilityMaxNumberOfCharacters)}";

            txt.Placeholder = $"{name} {suffix}".TrimEnd(',', ' ');
        }

        private void DrawContent()
        {
            _numberOfSeparators = 0;
            if (InputLength == 0)
                return;

            if (ChunkSize > InputLength)
                throw new ArgumentException("Property 'ChunkSize' must be smaller than 'InputLength'");
            _grid.Children.Clear();
            _grid.ColumnDefinitions.Clear();
            _grid.RowDefinitions!.Clear();
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var row = 0;
            var column = 0;

            for (int x = 0; x <= InputLength - 1; x++)
            {
                if (ChunkSize == 0 || x < ChunkSize)
                    _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = (_isPhone ? GridLength.Star : GridLength.Auto) });
                var tile = new PinTile(InputLength, x, true);

                tile.Clicked += async (s, e) => await Focus();
                _grid.Children.Add(tile, column, row);
                if (ChunkSize > 0 && ((x + 1) % ChunkSize) == 0 && ((x + 1) < InputLength))
                {
                    AddChunkSeparator(row, column);
                    row += ChunkOrientation == ChunkOrientationEnum.Vertical ? 2 : 0;
                    _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    column = ChunkOrientation == ChunkOrientationEnum.Vertical ? 0 : x + 2;
                }
                else
                {
                    column++;
                }
            }
            if (!string.IsNullOrEmpty(txt.Text))
            {
                for (int x = 0; x < txt.Text.Length; x++)
                {
                    HandleAdd(new TextChangedEventArgs(x == 0 ? "" : txt.Text.Substring(0, x), txt.Text.Substring(0, x + 1)));
                }
            }
        }

        private void AddChunkSeparator(int gridRow, int gridColumn)
        {
            var label = new CustomFontLabel()
            {
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                Style = (Style)Application.Current.Resources["LabelBold"],
                Text = "-",
            };

            if (ChunkOrientation == ChunkOrientationEnum.Vertical)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = 9 });
                label.VerticalTextAlignment = TextAlignment.Center;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, gridRow + 1);
                Grid.SetColumnSpan(label, ChunkSize);
            }
            else
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = (_isPhone ? GridLength.Star : GridLength.Auto) });
                label.VerticalTextAlignment = TextAlignment.Start;
                Grid.SetColumn(label, gridColumn + 1);
                Grid.SetRow(label, gridRow);
            }
            _numberOfSeparators++;
            _grid.Children.Add(label);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == null)
                return;

            if (propertyName.Equals(AutomationProperties.NameProperty.PropertyName) ||
                propertyName.Equals(nameof(InputLength)) ||
                propertyName.Equals(Entry.PlaceholderProperty.PropertyName))
                SetPlaceholder();
        }

        public new async Task Focus()
        {
            await Task.Delay(100);
            txt.Focus();
        }

        public async Task UnFocus()
        {
            await Task.Delay(100);
            txt.Unfocus();
        }

        private void HandleRemove(TextChangedEventArgs e)
        {
            var tileIndex = e.NewTextValue.Length;
            tileIndex += (ChunkSize <= 0 ? 0 : tileIndex / ChunkSize);

            if (tileIndex == InputLength + _numberOfSeparators)
                return;

            //backspace
            //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
            if (_grid.Children[tileIndex] is PinTile tile)
            {
                tile.Text = string.Empty;
                tile.State = PinTileState.Current;

                if (tileIndex >= InputLength + _numberOfSeparators - 1)
                    return;

                if (_grid.Children[e.NewTextValue.Length + 1] is PinTile nextTile)
                    nextTile.State = PinTileState.Inactive;
            }
        }

        private void HandleAdd(TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue.Substring(e.NewTextValue.Length - 1).ToUpperInvariant();

            var tileIndex = e.NewTextValue.Length - 1;
            tileIndex += (ChunkSize <= 0 ? 0 : tileIndex / ChunkSize);

            if (tileIndex == InputLength + _numberOfSeparators)
                return;

            //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
            if (_grid.Children[tileIndex] is PinTile tile)
            {
                tile.Text = newValue;
                tile.State = PinTileState.Active;

                if (tileIndex >= InputLength + _numberOfSeparators - 1)
                {
                    txt.Unfocus();
                    return;
                }
                if (_grid.Children[tileIndex + 1] is CustomFontLabel) // separator
                    tileIndex++;

                if (_grid.Children[tileIndex + 1] is PinTile current)
                    current.State = PinTileState.Current;
            }
        }

        private void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    var length = InputLength + _numberOfSeparators;
                    for (var x = 0; x < length; x++)
                    {
                        if (_grid.Children[x] is PinTile tile)
                        {
                            tile.Text = string.Empty;
                            tile.State = x == 0 ? PinTileState.Current : PinTileState.Inactive;
                        }
                    }
                }
                if (e.OldTextValue != null && e.NewTextValue.Length < e.OldTextValue.Length)
                    HandleRemove(e);
                else if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                        txt.Text = e.NewTextValue.ToUpperInvariant();
                    else
                        HandleAdd(e);
                }

                if (!DependencyService.Get<IA11YService>().IsInVoiceOverMode() && e.NewTextValue.Length == InputLength)
                    txt.Unfocus();
            }
            catch
            {
                //No logging needed    
            }
        }

    }
}
