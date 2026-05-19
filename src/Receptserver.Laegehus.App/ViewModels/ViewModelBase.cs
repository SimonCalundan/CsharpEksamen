using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Receptserver.Laegehus.App.ViewModels;

// Basisklasse til alle ViewModels. Implementerer INotifyPropertyChanged så UI
// automatisk opdateres når properties ændres. SetField hjælper med at undgå
// boilerplate i hver property-setter.
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
