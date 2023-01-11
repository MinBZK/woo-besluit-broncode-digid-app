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
using System.Windows.Input;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
    public class CustomEntryField : Grid, ICustomEntry
    {
        public static readonly BindableProperty ValidateCommandProperty = BindableProperty.Create(nameof(ValidateCommand), typeof(ICommand), typeof(CustomEntryField));
        public static readonly BindableProperty ValidateCommandArgumentsProperty = BindableProperty.Create(nameof(ValidateCommandArguments), typeof(string), typeof(CustomEntryField));
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntryField), default(string), BindingMode.TwoWay, propertyChanged: TextPropertyChanged);
        public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(nameof(ReturnCommand), typeof(ICommand), typeof(CustomEntryField), null, propertyChanged: ReturnCommandPropertyChanged);
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(CustomEntryField), true, BindingMode.TwoWay, propertyChanged: IsValidPropertyChanged);

        public ICommand ValidateCommand
        {
            get => (ICommand)GetValue(ValidateCommandProperty);
            set => SetValue(ValidateCommandProperty, value);
        }

        public string ValidateCommandArguments
        {
            get => (string)GetValue(ValidateCommandArgumentsProperty);
            set => SetValue(ValidateCommandArgumentsProperty, value);
        }

        private void HideCharCounterLabel()
        {
            CharCounterLabel.IsVisible = false;
            SetColumnSpan(HelperTextLabel, 3);
            SetColumnSpan(_cef_errorLabel, 3);
            ForceLayout();
        }

        private static void IsValidPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var isValid = (bool)newvalue;
            if (bindable is CustomEntryField entry)
            {
                entry.Frame.BackgroundColor = entry.Entry.IsFocused
                    ? (Color)Application.Current.Resources["CustomEntryFieldActiveBackgroundColor"]
                    : (Color)Application.Current.Resources["CustomEntryFieldBackgroundColor"];
                if (!isValid)
                {
                    entry.Frame.BorderColor = (Color)Application.Current.Resources["CustomEntryFieldFrameRequiredBorderColor"];
                    if (entry.TooltipFrame.IsVisible)
                    {
                        entry.TooltipFrame.IsVisible = false;
                        entry.TooltipFrameVisibilityChanged();
                    }

                    if (string.IsNullOrEmpty(entry.ErrorText) && !string.IsNullOrEmpty(entry.HelperText))
                        entry.ErrorLabel.ErrorText = entry.HelperText;
                    else if (!string.IsNullOrEmpty(entry.ErrorText))
                        entry.ErrorLabel.ErrorText = entry.ErrorText;

                    entry.ErrorLabel.IsVisible = entry.ErrorLabel.ErrorText?.Length > 0;

                    entry.HelperTextLabel.IsVisible = false;
                }
                else
                {
                    entry.Frame.BorderColor = (Color)Application.Current.Resources["CustomEntryFieldFrameBorderColor"];

                    entry.HelperTextLabel.Text = entry.HelperText;
                    entry.HelperTextLabel.IsVisible = !string.IsNullOrEmpty(entry.HelperText);
                    entry.ErrorLabel.IsVisible = false;
                }
            }
        }

        private bool _makePlaceholderInvisible;
        public bool MakePlaceholderInvisible
        {
            get => _makePlaceholderInvisible;
            set
            {
                _makePlaceholderInvisible = value;
                SetPlaceholderColor();
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string ErrorText
        {
            get => ErrorLabel.ErrorText;
            set => ErrorLabel.ErrorText = value;
        }

        public string HelperText
        {
            get => HelperTextLabel.Text;
            set
            {
                HelperTextLabel.Text = value;
                HelperTextLabel.IsVisible = value.Length > 0;
            }
        }

        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public string LabelText
        {
            get => Label.Text;
            set
            {
                Label.IsVisible = true;
                Label.Text = IsRequired ? value + " *" : value;
                Placeholder = value;
            }
        }

        public string Placeholder
        {
            get => Entry.Placeholder;
#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 22 to the 3 allowed.
            set
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 22 to the 3 allowed.
            {
                var name = AutomationProperties.GetName(this);
                var labeledBy = AutomationProperties.GetLabeledBy(this) as Label;

                var newValue = value?.TrimEnd('*', ' ');
                var labelText = "";

                if (labeledBy != null)
                    labelText = labeledBy.Text.Split('(')[0];
                else if (Label.Text != null)
                    labelText = Label.Text.Split('(')[0]; //zie ook CustomEntryRenderer.OnElement[Property]Changed()
                labelText = labelText.TrimEnd(' ', '*');

                // Omdat de tekst meestal langer is in VoiceOver/Talkback mode dan de echte placeholder,
                // zal de tekst niet (meer) passen daarom placeholder tekst invisible maken in VO/TB mode
                // en anders de waarde van de xaml overnemen.
                if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                {
                    MakePlaceholderInvisible = true;

                    var placeholder = newValue;

                    var labelAndValueAreEqual = newValue != null && labelText.Equals(newValue);

                    if (!string.IsNullOrEmpty(name))
                    {
                        placeholder = $"{labelText} , {name}";
                    }
                    else if (!labelAndValueAreEqual)
                    {
                        placeholder = $"{newValue} , {labelText}";
                    }

                    if (IsRequired)
                        placeholder += $", {AppResources.MandatoryInputField}";

                    if (!string.IsNullOrEmpty(HelperText))
                        placeholder += $", {HelperText}";

                    if (MinLength > 0 && string.IsNullOrEmpty(HelperText))
                        placeholder += $", {string.Format(AppResources.AccessibilityMinimalCharacters, MinLength)}";

                    if (MaxLength > 0)
                    {
                        placeholder += $", {string.Format(AppResources.AccessibilityMaximumCharacters, MaxLength)}";
                        if (!HideCharCounter && CharCounterLabel.IsVisible)
                            placeholder += $", {AutomationProperties.GetName(CharCounterLabel)}";
                    }

                    Entry.Placeholder = placeholder.TrimStart(',', ' ');
                    AutomationProperties.SetName(Entry, placeholder.TrimStart(',', ' '));
                }
                else
                {
                    Entry.Placeholder = newValue;
                }
            }
        }

        public ReturnType ReturnType
        {
            get => Entry.ReturnType;
            set => Entry.ReturnType = value;
        }

        public Keyboard Keyboard
        {
            get => Entry.Keyboard;
            set => Entry.Keyboard = value;
        }

        public bool IsPassword
        {
            get => Entry.IsPassword;
            set => Entry.IsPassword = value;
        }

        public bool IsRequired { get; set; } = true;

        public bool AutoUpperCase
        {
            get => Entry.TextTransform == TextTransform.Uppercase;
            set => Entry.TextTransform = value ? TextTransform.Uppercase : TextTransform.Default;
        }

        private int _maxLength;
        public int MaxLength
        {
            get => _maxLength;
            set
            {
                _maxLength = value;

                Entry.MaxLength = _maxLength;
                var showCounter = _maxLength > 0 && !HideCharCounter;

                if (showCounter)
                {
                    SetColumn(CharCounterLabel, 1);
                    SetColumnSpan(CharCounterLabel, string.IsNullOrEmpty(ToolTip) ? 2 : 1);
                    SetColumnSpan(HelperTextLabel, 1);
                    SetColumnSpan(_cef_errorLabel, 1);
                    CharCounterLabel.IsVisible = true;
                    UpdateCharCounter(-1);  // Initieel
                }
                else
                {
                    HideCharCounterLabel();
                }
                Placeholder = null; // forceer placeholder tekst aanpassing
            }
        }

        private int _minLength;
        public int MinLength
        {
            get => _minLength;
            set
            {
                _minLength = value;
                if (HideCharCounter)  // geen charcounter aanwezig
                {
                    SetColumnSpan(HelperTextLabel, string.IsNullOrEmpty(ToolTip) ? 3 : 2);
                }
                else
                {
                    SetColumnSpan(HelperTextLabel, 1);
                    SetColumnSpan(CharCounterLabel, string.IsNullOrEmpty(ToolTip) ? 2 : 1);
                }

                Placeholder = null; // forceer placeholder tekst aanpassing
            }
        }

        public string ToolTip
        {
            get => TooltipFrame.TooltipText;
            set
            {
                if (TooltipFrame.TooltipText == null)
                {
                    TooltipFrame.TooltipText = value;

                    CreateTooltipButton();

                    SetColumn(_tooltipButton, 2);
                    SetRow(_tooltipButton, 1);
                    Children.Add(_tooltipButton);
                    _tooltipButton.IsVisible = true;

                    var currentSpan = GetColumnSpan(Frame);
                    SetColumnSpan(Frame, currentSpan - 1);

                    SetColumnSpan(HelperTextLabel, 1);
                    SetColumnSpan(_cef_errorLabel, 1);
                    SetColumn(CharCounterLabel, 1);
                    SetColumnSpan(CharCounterLabel, 1);

                    ForceLayout();
                }
            }
        }

        private bool _hideCharCounter;
        public bool HideCharCounter
        {
            get => _hideCharCounter;
            set
            {
                _hideCharCounter = value;

                CharCounterLabel.IsVisible = !_hideCharCounter;
                if (MaxLength > 0 && CharCounterLabel.IsVisible && _hideCharCounter)
                {
                    HideCharCounterLabel();
                }

                ForceLayout();
            }
        }

        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null)
            {
                var ctrl = ((CustomEntryField)bindable);
                ctrl.Entry.Text = newValue.ToString();
            }
        }

        private static void ReturnCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CustomEntryField)bindable).Entry.ReturnCommand = (ICommand)newValue;
        }

        public Entry Entry { get; } = new Entry();
        public CustomFontLabel Label { get; } = new CustomFontLabel();
        public BorderedFrame Frame { get; }
        public TooltipFrame TooltipFrame { get; private set; }

        CustomFontLabel _helperTextLabel;
        public CustomFontLabel HelperTextLabel
        {
            get
            {
                if (_helperTextLabel == null)
                {
                    _helperTextLabel = new CustomFontLabel
                    {
                        IsVisible = false,
                        Style = (Style)Application.Current.Resources["LabelRegular"],
                        TextColor = (Color)Application.Current.Resources["CustomEntryFieldLabelTextColor"],
                        LineBreakMode = LineBreakMode.WordWrap
                    };
                }
                return _helperTextLabel;
            }
            private set => _helperTextLabel = value;
        }

        // Deze wordt standaard gebruikt door CustomEntryField. Om een errorlabel van buiten de 
        // CustomEntryField te gebruiken moet je dat via ErrorTextLabel property doen.
        readonly CustomErrorLabel _cef_errorLabel = new CustomErrorLabel { IsVisible = false, LineBreakMode = LineBreakMode.WordWrap };

        public CustomErrorLabel ErrorLabel => _cef_errorLabel;

        CustomFontLabel _charCounterLabel;
        public CustomFontLabel CharCounterLabel
        {
            get
            {
                if (_charCounterLabel is null)
                {
                    _charCounterLabel = new CustomFontLabel
                    {
                        IsVisible = false,
                        Style = (Style)Application.Current.Resources["LabelRegular"],
                        TextColor = (Color)Application.Current.Resources["CustomEntryFieldLabelTextColor"],
                        HorizontalOptions = LayoutOptions.End
                    };
                }
                return _charCounterLabel;
            }
            private set => _charCounterLabel = value;
        }

        private readonly DelegateWeakEventManager _completedEventManager = new DelegateWeakEventManager();

        public event EventHandler Completed
        {
            add => _completedEventManager.AddEventHandler(value);
            remove => _completedEventManager.RemoveEventHandler(value);
        }

        private ImageButton _tooltipButton;

        public CustomEntryField()
        {
            Padding = new Thickness(0);

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = 48 });
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            RowSpacing = 4;
            ColumnSpacing = 0;

            if (Device.RuntimePlatform == Device.Android)
                AutomationProperties.SetIsInAccessibleTree(this, false);

            SetColumn(Label, 0);
            SetColumnSpan(Label, 3);
            Label.TextColor = (Color)Application.Current.Resources["CustomEntryFieldLabelTextColor"];
            Label.FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label));
            Label.SetValue(AutomationProperties.IsInAccessibleTreeProperty, false);
            Label.Style = (Style)Application.Current.Resources["LabelBold"];
            Label.IsVisible = false;

            var padding = new Thickness(10, Device.RuntimePlatform == Device.iOS ? 0 : 2);
            Frame = new BorderedFrame
            {
                BorderColor = (Color)Application.Current.Resources["CustomEntryFieldFrameBorderColor"],
                BackgroundColor = (Color)Application.Current.Resources["CustomEntryFieldBackgroundColor"],
                CornerRadius = 5,
                Padding = padding,
                HasShadow = false,
                VerticalOptions = Device.RuntimePlatform == Device.Android ? LayoutOptions.FillAndExpand : LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            Frame.SetValue(AutomationProperties.IsInAccessibleTreeProperty, false);

            SetColumn(Frame, 0);
            SetColumnSpan(Frame, 3);
            SetRow(Frame, 1);

            AutomationProperties.SetLabeledBy(Entry, Label);

            Entry.TabIndex = TabIndex;
            Entry.TextColor = (Color)Application.Current.Resources[IsValid ? "CustomEntryFieldEntryTextColor" : "CustomEntryFieldFrameRequiredBorderColor"];
            Entry.FontFamily = "RO-Regular";
            Entry.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry));
            Entry.IsSpellCheckEnabled = false;
            Entry.IsTextPredictionEnabled = false;
            Entry.BackgroundColor = (Color)Application.Current.Resources[IsValid ? "CustomEntryFieldBackgroundColor" : ""];

            if (Device.RuntimePlatform == Device.Android)
            {
                Entry.VerticalOptions = LayoutOptions.EndAndExpand;
                Entry.VerticalTextAlignment = TextAlignment.End;
                Entry.HorizontalTextAlignment = TextAlignment.Start;
            }
            else
            {
                Entry.HeightRequest = 48;
                Entry.MinimumHeightRequest = 48;
            }

            Entry.HorizontalOptions = LayoutOptions.FillAndExpand;
            SetPlaceholderColor();

            Entry.Completed += Entry_Completed;
            Entry.TextChanged += Entry_TextChanged;
            Entry.Unfocused += EntryOnUnfocused;
            Entry.Focused += EntryOnFocused;

            SetColumn(HelperTextLabel, 0);
            SetColumnSpan(HelperTextLabel, 3);
            SetRow(HelperTextLabel, 2);

            SetColumn(_cef_errorLabel, 0);
            SetColumnSpan(_cef_errorLabel, 3);
            SetRow(_cef_errorLabel, 2);

            SetColumn(CharCounterLabel, 1);
            SetColumnSpan(CharCounterLabel, 2);
            SetRow(CharCounterLabel, 2);

            TooltipFrame = new TooltipFrame();
            TooltipFrame.IsVisible = false;
            AutomationProperties.SetLabeledBy(TooltipFrame, Label);

            SetColumn(TooltipFrame, 0);
            SetColumnSpan(TooltipFrame, 3);
            SetRow(TooltipFrame, 2);

            Children.Add(Label);
            Frame.Content = Entry;
            Children.Add(Frame);
            Children.Add(_cef_errorLabel);
            Children.Add(HelperTextLabel);
            Children.Add(CharCounterLabel);
            Children.Add(TooltipFrame);
        }

        ~CustomEntryField()
        {
            Entry.Completed -= Entry_Completed;
            Entry.TextChanged -= Entry_TextChanged;
            Entry.Focused -= EntryOnFocused;
            Entry.Unfocused -= EntryOnUnfocused;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Label) || propertyName == nameof(ToolTip))
            {
                UpdateTooltipAutomationProperties();
            }
            else if (AutomationProperties.NameProperty.PropertyName.Equals(propertyName) && DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                Placeholder = AutomationProperties.GetName(this);
            else if (propertyName == nameof(WidthRequest))
            {
                Frame.WidthRequest = WidthRequest - (_tooltipButton != null ? 48 : 0);
                ColumnDefinitions[0].Width = new GridLength(Frame.WidthRequest, GridUnitType.Absolute);
                SetColumnSpan(Frame, 1);
                ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Absolute);
                ColumnDefinitions[2].Width = new GridLength(48, GridUnitType.Absolute);
                ForceLayout();
            }
        }

        public bool CheckMinimum(CustomEntryField entryIfValid = null)
        {
            if (MinLength > 0 && Entry.Text.Length < MinLength)
            {
                IsValid = false;
                return false;
            }

            entryIfValid?.Focus();
            return true;
        }

        private void EntryOnFocused(object sender, FocusEventArgs e)
        {
            if (e.IsFocused)
            {
                Entry.BackgroundColor = (Color)Application.Current.Resources["CustomEntryFieldActiveBackgroundColor"];
                if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                    Placeholder = null; //forceer update placeholder voor a11y.
                var a11yText = AutomationProperties.GetName(Entry);
                if (!IsValid && !string.IsNullOrEmpty(ErrorLabel.ErrorText))
                    DependencyService.Get<IA11YService>().Speak($"{AppResources.AccessibilityErrorLabelWarning}, {ErrorLabel.ErrorText}, {Entry.Text}");
                else
                    DependencyService.Get<IA11YService>().Speak(a11yText);

                Frame.BackgroundColor = (Color)Application.Current.Resources["CustomEntryFieldActiveBackgroundColor"];
                Frame.BorderColor = (Color)Application.Current.Resources[IsValid ? "CustomEntryFieldFrameBorderColor" : "CustomEntryFieldFrameRequiredBorderColor"];
            }
        }

        private void EntryOnUnfocused(object sender, FocusEventArgs e)
        {
            if (!e.IsFocused)
            {
                if (ValidateCommand?.CanExecute(ValidateCommandArguments) == true)
                    ValidateCommand.Execute(ValidateCommandArguments);

                Frame.BackgroundColor = (Color)Application.Current.Resources[IsValid ? "CustomEntryFieldActiveBackgroundColor" : "CustomEntryFieldBackgroundColor"];
                Frame.BorderColor = (Color)Application.Current.Resources[IsValid ? "CustomEntryFieldFrameBorderColor" : "CustomEntryFieldFrameRequiredBorderColor"];

                Entry.BackgroundColor = Frame.BackgroundColor;

                ErrorLabel.IsVisible = !IsValid && !string.IsNullOrEmpty(ErrorText);
                HelperTextLabel.IsVisible = IsValid && !string.IsNullOrEmpty(HelperText);
            }
        }

        protected void SetPlaceholderColor()
        {
            if (_makePlaceholderInvisible)
                Entry.PlaceholderColor = Color.Transparent;
            else
            {
                Entry.PlaceholderColor = (Color)Application.Current.Resources["CustomEntryFieldPlaceholderColor"]; //Gray7
                Entry.FontAttributes = FontAttributes.Italic;
            }
        }

        public new void Focus()
        {
            Entry.Focus();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            if ((IsRequired && string.IsNullOrEmpty(Text)) || !IsValid)
                Frame.BorderColor = (Color)Application.Current.Resources["CustomEntryFieldFrameRequiredBorderColor"];
            else
                Frame.BorderColor = (Color)Application.Current.Resources["CustomEntryFieldFrameBorderColor"];

            _completedEventManager.RaiseEvent(this, e, nameof(Completed));
        }

        private void HandleNumericInput(object sender, TextChangedEventArgs e)
        {
            if (Text.Length == MaxLength)
            {
                Entry_Completed(sender, e); // checken op validiteit
                ReturnCommand?.Execute(null);
            }
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        {
            if (!string.IsNullOrEmpty(e.OldTextValue) && !IsValid)
            {
                //er wordt een foutboodschap getoond, deze kan nu weg
                ErrorLabel.IsVisible = false;
                HelperTextLabel.IsVisible = !string.IsNullOrEmpty(HelperTextLabel.Text);
            }

            Text = e.NewTextValue;

            if (MaxLength > 0 && !HideCharCounter)
                UpdateCharCounter(e.NewTextValue.Length);

            if (Keyboard == Keyboard.Numeric)
            {
                HandleNumericInput(sender, e);
                return;
            }

            if (IsRequired && string.IsNullOrEmpty(Text))
            {
                if (!string.IsNullOrEmpty(e.OldTextValue))
                    IsValid = false;

                return;
            }

            if (Text.Length == MaxLength)
            {
                Entry_Completed(sender, e); // Zorgt ervoor dat de border de juiste kleur krijgt.
                await DependencyService.Get<IA11YService>().Speak(AppResources.AccessibilityEnteredMaxCharacters, 500);
            }
        }

#pragma warning disable S3168 //Bug: Return 'Task' instead. 
        private async void UpdateCharCounter(int length)
#pragma warning restore S3168 //Bug.
        {
            var speak = length >= 0;
            var newLength = speak ? length : 0;
            CharCounterLabel.Text = string.Format(AppResources.AccessibilityEnteredCharacters, newLength, MaxLength);
            AutomationProperties.SetName(CharCounterLabel, $"{newLength} {AppResources.AccessibilityOf} {MaxLength}");
            if (speak)
                await DependencyService.Get<IA11YService>().Speak($"{newLength} {AppResources.AccessibilityOf} {MaxLength}", 500);

            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                Placeholder = null; //forceer update
        }

        private void CreateTooltipButton()
        {
            _tooltipButton = new ImageButton
            {
                Padding = new Thickness(12, 0, 12, 24),
                Aspect = Aspect.AspectFit,
                BackgroundColor = Color.Transparent,
                IsEnabled = true
            };

            _tooltipButton.SetOnAppTheme<FileImageSource>(Image.SourceProperty, "digid_icon_tooltip.png", "digid_icon_tooltip_dark.png");

            AutomationProperties.SetIsInAccessibleTree(_tooltipButton, true);
            AutomationProperties.SetName(_tooltipButton, string.Format(AppResources.AccessibilityTooltipNotShowing, GetTooltipPrefix()));

            SetTooltipButtonCommand();

            _tooltipButton.HeightRequest = 48;
            _tooltipButton.WidthRequest = 24;
        }

        private void SetTooltipButtonCommand()
        {
            _tooltipButton.Command = new Command(() =>
            {
                TooltipFrame.IsVisible = !TooltipFrame.IsVisible;

                TooltipFrameVisibilityChanged();

                UpdateTooltipAutomationProperties();
            });
        }

        public void TooltipFrameVisibilityChanged()
        {
            if (TooltipFrame.IsVisible)
            {
                HelperTextLabel.TranslateTo(10, 20, 250, Easing.Linear);
                ErrorLabel.TranslateTo(10, 20, 250, Easing.Linear);
                CharCounterLabel.TranslateTo(0, 20, 250, Easing.Linear);
                HelperTextLabel.IsVisible = ErrorLabel.IsVisible = CharCounterLabel.IsVisible = false;
            }
            else
            {
                HelperTextLabel.IsVisible = !string.IsNullOrEmpty(HelperTextLabel.Text) && IsValid;
                ErrorLabel.IsVisible = !string.IsNullOrEmpty(ErrorLabel.ErrorText) && !IsValid;
                CharCounterLabel.IsVisible = MaxLength > 0 && !HideCharCounter;
                HelperTextLabel.TranslateTo(0, 0, 250, Easing.Linear);
                ErrorLabel.TranslateTo(0, 0, 250, Easing.Linear);
                CharCounterLabel.TranslateTo(0, 0, 250, Easing.Linear);
            }
        }

        private void UpdateTooltipAutomationProperties()
        {
            if (_tooltipButton != null)
            {
                if (TooltipFrame.IsVisible)
                {
                    DependencyService.Get<IA11YService>().Speak(TooltipFrame.TooltipText, 2000);
                    AutomationProperties.SetName(_tooltipButton, string.Format(AppResources.AccessibilityTooltipShowing, GetTooltipPrefix()));
                }
                else
                    AutomationProperties.SetName(_tooltipButton, string.Format(AppResources.AccessibilityTooltipNotShowing, GetTooltipPrefix()));
            }
        }

        private string GetTooltipPrefix()
        {
            string result = string.Empty;

            if (AutomationProperties.GetLabeledBy(TooltipFrame) is CustomFontLabel cfl)
            {
                if (!string.IsNullOrEmpty(cfl.Text))
                    result = $"{cfl.Text.Split('(')[0]}";
                result = result.TrimStart(' ', ',');
            }

            return result.TrimEnd('*', ' ');
        }
    }
}
