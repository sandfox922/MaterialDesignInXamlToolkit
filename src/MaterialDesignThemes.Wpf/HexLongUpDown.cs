#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexLongUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<long>
#else
    : UpDownBase<long, HexlongArithmetic>
#endif
{
    static HexLongUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexLongUpDown), new FrameworkPropertyMetadata(typeof(HexLongUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexLongUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexlongArithmetic : IArithmetic<long>
{
    public long Add(long value1, long value2) => value1 + value2;

    public long Subtract(long value1, long value2) => value1 - value2;

    public int Compare(long value1, long value2) => value1.CompareTo(value2);

    public long MinValue() => long.MinValue;

    public long MaxValue() => long.MaxValue;

    public long One() => 1;

    public long Max(long value1, long value2) => Math.Max(value1, value2);

    public long Min(long value1, long value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out long value)
        => long.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
