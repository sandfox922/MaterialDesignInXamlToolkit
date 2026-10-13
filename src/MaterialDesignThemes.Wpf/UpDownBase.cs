using System.ComponentModel;
using System.Globalization;

namespace MaterialDesignThemes.Wpf;

using System.Diagnostics.CodeAnalysis;


#if NET8_0_OR_GREATER
using System.Numerics;
public class UpDownBase<T> : UpDownBase
    where T : INumber<T>, IMinMaxValue<T>
{
    private static readonly Type SelfType = typeof(UpDownBase<T>);

    private static UpDownBase<T> ToUpDownBase(DependencyObject dependencyObject) => (UpDownBase<T>)dependencyObject;

    private static T MinValue => T.MinValue;
    private static T MaxValue => T.MaxValue;
    private static T One => T.One;
    private static T Max(T value1, T value2) => T.Max(value1, value2);
    private static T Clamp(T value, T min, T max) => T.Clamp(value, min, max);
    private static T Add(T value1, T value2) => value1 + value2;
    private static T Subtract(T value1, T value2) => value1 - value2;
    private static bool TryParse(string text, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T value)
        => T.TryParse(text, formatProvider, out value);
    private static int Compare(T value1, T value2) => value1.CompareTo(value2);
#else
public class UpDownBase<T, TArithmetic> : UpDownBase
    where TArithmetic : IArithmetic<T>, new()
{
    private static readonly Type SelfType = typeof(UpDownBase<T, TArithmetic>);
    private static readonly TArithmetic _arithmetic = new();

    private static UpDownBase<T, TArithmetic> ToUpDownBase(DependencyObject dependencyObject) => (UpDownBase<T, TArithmetic>)dependencyObject;

    private static T MinValue => _arithmetic.MinValue();
    private static T MaxValue => _arithmetic.MaxValue();
    private static T One => _arithmetic.One();
    private static T Max(T value1, T value2) => _arithmetic.Max(value1, value2);
    private static T Clamp(T value, T min, T max) => _arithmetic.Max(_arithmetic.Min(value, max), min);
    private static T Add(T value1, T value2) => _arithmetic.Add(value1, value2);
    private static T Subtract(T value1, T value2) => _arithmetic.Subtract(value1, value2);
    private static bool TryParse(string text, IFormatProvider? formatProvider, [NotNullWhen(true)] out T value)
        => _arithmetic.TryParse(text, formatProvider, out value);

    private static int Compare(T value1, T value2) => _arithmetic.Compare(value1, value2);
#endif

    private bool _isTextChanged;

    #region DependencyProperties

    #region DependencyProperty : MinimumProperty

    public virtual T Minimum
    {
        get => (T)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(T), SelfType, new PropertyMetadata(MinValue, OnMinimumChanged));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = ToUpDownBase(d);
        ctrl.CoerceValue(MaximumProperty);
        ctrl.CoerceValue(ValueProperty);
    }

    #endregion DependencyProperty : MinimumProperty

    #region DependencyProperty : MaximumProperty

    public T Maximum
    {
        get => (T)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(T), SelfType, new PropertyMetadata(MaxValue, OnMaximumChanged, CoerceMaximum));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var upDownBase = ToUpDownBase(d);
        upDownBase.CoerceValue(ValueProperty);
    }

    private static object? CoerceMaximum(DependencyObject d, object? value)
    {
        if (value is T numericValue)
        {
            var upDownBase = ToUpDownBase(d);
            return Max(upDownBase.Minimum, numericValue);
        }
        return value;
    }

    #endregion DependencyProperty : MaximumProperty

    #region DependencyProperty : ValueProperty
    public T Value
    {
        get => (T)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(T), SelfType, new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnNumericValueChanged, CoerceNumericValue));

    private static void OnNumericValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var upDownBase = ToUpDownBase(d);
        var args = new RoutedPropertyChangedEventArgs<T>((T)e.OldValue, (T)e.NewValue)
        {
            RoutedEvent = ValueChangedEvent
        };
        upDownBase.RaiseEvent(args);

        if (upDownBase._textBoxField is { } textBox)
        {
            textBox.Text = upDownBase.FormatValue((T)e.NewValue);
        }

        upDownBase.UpdateDecreaseButtonEnabled();
        upDownBase.UpdateIncreaseButtonEnabled();
    }

    private void UpdateIncreaseButtonEnabled()
    {
        if (_increaseButton is { } increaseButton)
        {
            increaseButton.IsEnabled = Compare(Value, Maximum) < 0;
        }
    }

    private void UpdateDecreaseButtonEnabled()
    {
        if (_decreaseButton is { } decreaseButton)
        {
            decreaseButton.IsEnabled = Compare(Value, Minimum) > 0;
        }
    }

    private static object? CoerceNumericValue(DependencyObject d, object? value)
    {
        if (value is T numericValue)
        {
            var upDownBase = ToUpDownBase(d);
            numericValue = upDownBase.ClampValue(numericValue);
            return numericValue;
        }
        return value;
    }
    #endregion ValueProperty

    #region DependencyProperty : ValueStep
    /// <summary>
    /// The step of value for each increase or decrease
    /// </summary>
    public T ValueStep
    {
        get => (T)GetValue(ValueStepProperty);
        set => SetValue(ValueStepProperty, value);
    }

    public static readonly DependencyProperty ValueStepProperty =
        DependencyProperty.Register(nameof(ValueStep), typeof(T), SelfType, new PropertyMetadata(One));
    #endregion

    #region DependencyProperty : AllowChangeOnScroll

    public bool AllowChangeOnScroll
    {
        get => (bool)GetValue(AllowChangeOnScrollProperty);
        set => SetValue(AllowChangeOnScrollProperty, value);
    }

    public static readonly DependencyProperty AllowChangeOnScrollProperty =
        DependencyProperty.Register(nameof(AllowChangeOnScroll), typeof(bool), SelfType, new PropertyMetadata(false));

    #endregion

    #region DependencyProperty : SelectAllOnFocus
    public bool SelectAllOnFocus
    {
        get => (bool)GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    public static readonly DependencyProperty SelectAllOnFocusProperty =
        DependencyProperty.Register(nameof(SelectAllOnFocus), typeof(bool), SelfType, new PropertyMetadata(false));
    #endregion

    #region DependencyProperty : DecimalPlaces
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    public static readonly DependencyProperty DecimalPlacesProperty =
        DependencyProperty.Register(nameof(DecimalPlaces), typeof(int), SelfType, new PropertyMetadata(-1, OnDecimalPlacesChanged, OnCoerceDecimalPlaces));

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.CoerceValue(ValueProperty);

        var upDownBase = ToUpDownBase(d);
        upDownBase.UpdateValue();
    }

    private static object OnCoerceDecimalPlaces(DependencyObject d, object baseValue)
    {
        int value = (int)baseValue;
        if (value > 28) return 28;
        return value;
    }
    #endregion

    #region DependencyProperty : Hexadecimal
    protected bool Hexadecimal
    {
        get => (bool)GetValue(HexadecimalProperty);
        set => SetValue(HexadecimalProperty, value);
    }

    protected static readonly DependencyProperty HexadecimalProperty =
        DependencyProperty.Register(nameof(Hexadecimal), typeof(bool), SelfType, new PropertyMetadata(false));

    #endregion

    #endregion DependencyProperties

    #region Event : ValueChangedEvent
    [Category("Behavior")]
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<T>), SelfType);

    public event RoutedPropertyChangedEventHandler<T> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }
    #endregion Event : ValueChangedEvent

    public override void OnApplyTemplate()
    {
        if (_increaseButton != null)
        {
            _increaseButton.Click -= IncreaseButtonOnClick;
        }
        if (_decreaseButton != null)
        {
            _decreaseButton.Click -= DecreaseButtonOnClick;
        }
        if (_textBoxField != null)
        {
            _textBoxField.GotKeyboardFocus -= OnTextBoxGotKeyboardFocus;
            _textBoxField.PreviewMouseDown -= OnTextBoxPreviewMouseDown;
            _textBoxField.PreviewTextInput -= OnTextBoxPreviewTextInput;
            _textBoxField.TextChanged -= OnTextBoxTextChanged;
            _textBoxField.LostFocus -= OnTextBoxLostFocus;
        }

        base.OnApplyTemplate();

        if (_increaseButton != null)
        {
            _increaseButton.Click += IncreaseButtonOnClick;
            UpdateIncreaseButtonEnabled();
        }

        if (_decreaseButton != null)
        {
            _decreaseButton.Click += DecreaseButtonOnClick;
            UpdateDecreaseButtonEnabled();
        }

        if (_textBoxField != null)
        {
            _textBoxField.GotKeyboardFocus += OnTextBoxGotKeyboardFocus;
            _textBoxField.PreviewMouseDown += OnTextBoxPreviewMouseDown;
            _textBoxField.PreviewTextInput += OnTextBoxPreviewTextInput;
            _textBoxField.TextChanged += OnTextBoxTextChanged;
            _textBoxField.LostFocus += OnTextBoxLostFocus;
            _textBoxField.Text = FormatValue(Value);
        }

    }

    private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (e == null) return;

        var numberFormat = CultureInfo.CurrentCulture.NumberFormat;

        // Restrict input keys
        if (_textBoxField is { } textBoxField)
        {
            // For hexadecimal, validate HEX digits
            if (Hexadecimal)
            {
                if (e.Text.Length > 0 && !Uri.IsHexDigit(e.Text[0]))
                {
                    e.Handled = true;
                    return;
                }
            }
            // Otherwise, allow/control sign, decimal separator, and digits
            else
            {
                if (e.Text == numberFormat.NegativeSign)
                {
                    if (textBoxField.CaretIndex != 0 || textBoxField.Text.IndexOf(numberFormat.NegativeSign, StringComparison.CurrentCulture) > -1)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else if (e.Text == numberFormat.NumberDecimalSeparator)
                {
                    if (textBoxField.Text.IndexOf(numberFormat.NumberDecimalSeparator, StringComparison.CurrentCulture) > -1)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else if (e.Text.Length > 0 && !char.IsDigit(e.Text[0]))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }

    private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (SelectAllOnFocus) SelectAll();
    }

    private void OnTextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // When SelectAllOnFocus is enabled and the text box does not have focus,
        // focus the text area when interacting outside the UpDown buttons.
        if (SelectAllOnFocus
            && _textBoxField?.IsFocused == false
            && e.Source is not ContentPresenter)
        {
            _textBoxField.Focus();
            e.Handled = true;
        }
    }

    private void OnTextBoxLostFocus(object sender, EventArgs e)
    {
        UpdateValue();
    }

    private void OnTextBoxTextChanged(object sender, EventArgs e)
    {
        _isTextChanged = true;
    }

    private void UpdateValue()
    {
        if (_isTextChanged && _textBoxField is { } textBoxField)
        {
            // Convert hexadecimal string to number (excluding double/decimal/float)
            if (Hexadecimal && typeof(T) != typeof(double) && typeof(T) != typeof(decimal) && typeof(T) != typeof(float))
            {
                if (ulong.TryParse(textBoxField.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out ulong ulongVal))
                {
                    try
                    {
                        var value = (T)Convert.ChangeType(ulongVal, typeof(T), CultureInfo.CurrentCulture);
                        SetCurrentValue(ValueProperty, ClampValue(value));
                    }
                    catch
                    {
                    }
                }
            }
            // Convert regular string to number
            else
            {
                if (TryParse(textBoxField.Text, CultureInfo.CurrentCulture, out T? value))
                {
                    value = ApplyDecimalPlaces(value);
                    SetCurrentValue(ValueProperty, ClampValue(value));
                }
            }
            // Although not updated, this is to eliminate display irregularities.
            _textBoxField.Text = FormatValue(Value);
            _isTextChanged = false;
        }
    }

    private string FormatValue(T value)
    {
        int digits = DecimalPlaces;
        string format;

        // Create format for hexadecimal (excluding double/decimal/float)
        if (Hexadecimal && value is not (double or decimal or float))
        {
            format = digits <= 0 ? "X" : $"X{digits}";
        }
        // Create decimal format (negative digit count is invalid)
        else
        {
            if (digits < 0)
            {
                return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
            }
            format = $"F{digits}";
        }

        // If value supports formatting, apply the format
        if (value is IFormattable formattable)
        {
            return formattable.ToString(format, CultureInfo.CurrentCulture);
        }
        // Fallback
        return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
    }

    private T ApplyDecimalPlaces(T value)
    {
        // Round decimal places
        int digits = DecimalPlaces;
        if (digits < 0) return value;

        return value switch
        {
            double d => (T)(object)Math.Round(d, digits),
            decimal d => (T)(object)Math.Round(d, digits),
            float d => (T)(object)Math.Round(d, digits),
            _ => value
        };
    }

    private void IncreaseButtonOnClick(object sender, RoutedEventArgs e) => OnIncrease();

    private void DecreaseButtonOnClick(object sender, RoutedEventArgs e) => OnDecrease();

    private void OnIncrease()
    {
        UpdateValue();
        SetCurrentValue(ValueProperty, Clamp(Add(Value, ValueStep), Minimum, Maximum));
    }

    private void OnDecrease()
    {
        UpdateValue();
        SetCurrentValue(ValueProperty, Clamp(Subtract(Value, ValueStep), Minimum, Maximum));
    }

    private T ClampValue(T value)
        => Clamp(value, Minimum, Maximum);

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            OnIncrease();
            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            OnDecrease();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            // On Enter key press, update the value and select all text
            UpdateValue();
            if (SelectAllOnFocus) SelectAll();
            e.Handled = true;
        }
        base.OnPreviewKeyDown(e);
    }

    protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
    {
        if (IsKeyboardFocusWithin && AllowChangeOnScroll)
        {
            if (e.Delta > 0)
            {
                OnIncrease();
            }
            else if (e.Delta < 0)
            {
                OnDecrease();
            }
            e.Handled = true;
        }
        base.OnPreviewMouseWheel(e);
    }
}

[TemplatePart(Name = IncreaseButtonPartName, Type = typeof(RepeatButton))]
[TemplatePart(Name = DecreaseButtonPartName, Type = typeof(RepeatButton))]
[TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
public class UpDownBase : Control
{
    #region DependencyProperties

    #region DependencyProperty : IsEditable
    public bool IsEditable
    {
        get => (bool)GetValue(IsEditableProperty);
        set => SetValue(IsEditableProperty, value);
    }

    public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(UpDownBase), new PropertyMetadata(true));

    #endregion

    #endregion DependencyProperties

    public const string IncreaseButtonPartName = "PART_IncreaseButton";
    public const string DecreaseButtonPartName = "PART_DecreaseButton";
    public const string TextBoxPartName = "PART_TextBox";

    protected TextBox? _textBoxField;
    protected RepeatButton? _decreaseButton;
    protected RepeatButton? _increaseButton;

    static UpDownBase()
    {
        EventManager.RegisterClassHandler(typeof(UpDownBase), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
    }

    // Based on work in MahApps
    // https://github.com/MahApps/MahApps.Metro/blob/f7ba30586e9670f07c2f7b6553d129a9e32fc673/src/MahApps.Metro/Controls/NumericUpDown.cs#L966
    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
        // When NumericUpDown gets logical focus, select the text inside us.
        // If we're an editable NumericUpDown, forward focus to the TextBox element
        if (!e.Handled)
        {
            var numericUpDown = (UpDownBase)sender;
            if (numericUpDown.Focusable && e.OriginalSource == numericUpDown)
            {
                // MoveFocus takes a TraversalRequest as its argument.
                var focusDirection = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                    ? FocusNavigationDirection.Previous
                    : FocusNavigationDirection.Next;

                var request = new TraversalRequest(focusDirection);
                // Gets the element with keyboard focus.
                // And change the keyboard focus.
                if (Keyboard.FocusedElement is UIElement elementWithFocus)
                {
                    elementWithFocus.MoveFocus(request);
                }
                else
                {
                    numericUpDown.Focus();
                }

                e.Handled = true;
            }
        }
    }

    public override void OnApplyTemplate()
    {
        _increaseButton = GetTemplateChild(IncreaseButtonPartName) as RepeatButton;
        _decreaseButton = GetTemplateChild(DecreaseButtonPartName) as RepeatButton;
        _textBoxField = GetTemplateChild(TextBoxPartName) as TextBox;

        base.OnApplyTemplate();
    }

    public void SelectAll() => _textBoxField?.SelectAll();

    public object? IncreaseContent
    {
        get => GetValue(IncreaseContentProperty);
        set => SetValue(IncreaseContentProperty, value);
    }

    // Using a DependencyProperty as the backing store for IncreaseContent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IncreaseContentProperty =
        DependencyProperty.Register(nameof(IncreaseContent), typeof(object), typeof(UpDownBase), new PropertyMetadata(null));

    public object? DecreaseContent
    {
        get => GetValue(DecreaseContentProperty);
        set => SetValue(DecreaseContentProperty, value);
    }

    // Using a DependencyProperty as the backing store for DecreaseContent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DecreaseContentProperty =
        DependencyProperty.Register(nameof(DecreaseContent), typeof(object), typeof(UpDownBase), new PropertyMetadata(null));

}

#if !NET8_0_OR_GREATER
public interface IArithmetic<T>
{
    T Add(T value1, T value2);

    T Subtract(T value1, T value2);

    int Compare(T value1, T value2);

    T MinValue();

    T MaxValue();

    T One();

    T Max(T value1, T value2);

    T Min(T value1, T value2);

    bool TryParse(string text, IFormatProvider? formatProvider, [NotNullWhen(true)] out T value);
}
#endif
