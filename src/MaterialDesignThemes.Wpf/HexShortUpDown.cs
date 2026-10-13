#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexShortUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<short>
#else
    : UpDownBase<short, HexshortArithmetic>
#endif
{
    static HexShortUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexShortUpDown), new FrameworkPropertyMetadata(typeof(HexShortUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexShortUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexshortArithmetic : IArithmetic<short>
{
    public short Add(short value1, short value2) => (short)(value1 + value2);

    public short Subtract(short value1, short value2) => (short)(value1 - value2);

    public int Compare(short value1, short value2) => value1.CompareTo(value2);

    public short MinValue() => short.MinValue;

    public short MaxValue() => short.MaxValue;

    public short One() => 1;

    public short Max(short value1, short value2) => Math.Max(value1, value2);

    public short Min(short value1, short value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out short value)
        => short.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
