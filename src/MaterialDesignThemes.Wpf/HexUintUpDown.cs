#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexUintUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<uint>
#else
    : UpDownBase<uint, HexuintArithmetic>
#endif
{
    static HexUintUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexUintUpDown), new FrameworkPropertyMetadata(typeof(HexUintUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexUintUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexuintArithmetic : IArithmetic<uint>
{
    public uint Add(uint value1, uint value2) => value1 + value2;

    public uint Subtract(uint value1, uint value2) => value1 - value2;

    public int Compare(uint value1, uint value2) => value1.CompareTo(value2);

    public uint MinValue() => uint.MinValue;

    public uint MaxValue() => uint.MaxValue;

    public uint One() => 1;

    public uint Max(uint value1, uint value2) => Math.Max(value1, value2);

    public uint Min(uint value1, uint value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out uint value)
        => uint.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
