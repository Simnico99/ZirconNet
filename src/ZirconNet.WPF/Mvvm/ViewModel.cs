using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ZirconNet.WPF.Mvvm;
public abstract class ViewModel : ObservableObject 
{
    public void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
