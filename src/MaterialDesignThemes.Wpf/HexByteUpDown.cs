#if !NET8_0_OR_GREATER
using System.Globalization;
#endif

namespace MaterialDesignThemes.Wpf;

public class HexByteUpDown
#if NET8_0_OR_GREATER
    : UpDownBase<byte>
#else
    : UpDownBase<byte, HexByteArithmetic>
#endif
{
    static HexByteUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexByteUpDown), new FrameworkPropertyMetadata(typeof(HexByteUpDown)));

        HexadecimalProperty.OverrideMetadata(typeof(HexByteUpDown), new PropertyMetadata(true));
    }
}

#if !NET8_0_OR_GREATER
public class HexByteArithmetic : IArithmetic<byte>
{
    public byte Add(byte value1, byte value2) => (byte)(value1 + value2);

    public byte Subtract(byte value1, byte value2) => (byte)(value1 - value2);

    public int Compare(byte value1, byte value2) => value1.CompareTo(value2);

    public byte MinValue() => byte.MinValue;

    public byte MaxValue() => byte.MaxValue;

    public byte One() => 1;

    public byte Max(byte value1, byte value2) => Math.Max(value1, value2);

    public byte Min(byte value1, byte value2) => Math.Min(value1, value2);

    public bool TryParse(string text, IFormatProvider? formatProvider, out byte value)
        => byte.TryParse(text, NumberStyles.HexNumber, formatProvider, out value);
}
#endif
