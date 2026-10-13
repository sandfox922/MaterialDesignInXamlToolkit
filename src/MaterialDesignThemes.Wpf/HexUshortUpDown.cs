#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexUshortUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<ushort>
#else
    : UpDownBase<ushort, HexushortArithmetic>
#endif
{
    static HexUshortUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexUshortUpDown), new FrameworkPropertyMetadata(typeof(HexUshortUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexUshortUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexushortArithmetic : IArithmetic<ushort>
{
    public ushort Add(ushort value1, ushort value2) => (ushort)(value1 + value2);

    public ushort Subtract(ushort value1, ushort value2) => (ushort)(value1 - value2);

    public int Compare(ushort value1, ushort value2) => value1.CompareTo(value2);

    public ushort MinValue() => ushort.MinValue;

    public ushort MaxValue() => ushort.MaxValue;

    public ushort One() => 1;

    public ushort Max(ushort value1, ushort value2) => Math.Max(value1, value2);

    public ushort Min(ushort value1, ushort value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out ushort value)
        => ushort.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
