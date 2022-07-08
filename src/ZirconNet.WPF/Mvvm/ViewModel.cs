using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ZirconNet.WPF.Mvvm;
public class ViewModel : ObservableObject 
{
    public void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
