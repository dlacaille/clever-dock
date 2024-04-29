using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CleverDock.Patterns;

public class ViewModelBase : INotifyPropertyChanged
{
    private bool? _isInDesignMode;

    /// <summary>
    ///     Gets a value indicating whether the control is in design mode.
    /// </summary>
    protected bool IsInDesignMode
    {
        get
        {
            if (!_isInDesignMode.HasValue)
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                _isInDesignMode = (bool)DependencyPropertyDescriptor
                    .FromProperty(prop, typeof(FrameworkElement))
                    .Metadata.DefaultValue;
            }

            return _isInDesignMode.Value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, e);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}