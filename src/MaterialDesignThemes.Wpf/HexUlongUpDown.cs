#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexUlongUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<ulong>
#else
    : UpDownBase<ulong, HexulongArithmetic>
#endif
{
    static HexUlongUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexUlongUpDown), new FrameworkPropertyMetadata(typeof(HexUlongUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexUlongUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexulongArithmetic : IArithmetic<ulong>
{
    public ulong Add(ulong value1, ulong value2) => value1 + value2;

    public ulong Subtract(ulong value1, ulong value2) => value1 - value2;

    public int Compare(ulong value1, ulong value2) => value1.CompareTo(value2);

    public ulong MinValue() => ulong.MinValue;

    public ulong MaxValue() => ulong.MaxValue;

    public ulong One() => 1;

    public ulong Max(ulong value1, ulong value2) => Math.Max(value1, value2);

    public ulong Min(ulong value1, ulong value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out ulong value)
        => ulong.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
