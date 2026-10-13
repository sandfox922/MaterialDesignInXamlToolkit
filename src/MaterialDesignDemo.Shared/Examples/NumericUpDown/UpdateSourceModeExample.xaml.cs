using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaterialDesignDemo.Shared.Examples.NumericUpDown;

public partial class UpdateSourceModeExample : UserControl, INotifyPropertyChanged
{
    public UpdateSourceModeExample()
    {
        InitializeComponent();
    }

    private decimal _valuePropertyChanged = 5;
    public decimal ValuePropertyChanged
    {
        get => _valuePropertyChanged;
        set
        {
            if (_valuePropertyChanged != value)
            {
                _valuePropertyChanged = value;
                OnPropertyChanged();
            }
        }
    }

    private double _valueLostFocus = 2.5;
    public double ValueLostFocus
    {
        get => _valueLostFocus;
        set
        {
            if (_valueLostFocus != value)
            {
                _valueLostFocus = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property, used to notify listeners.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
