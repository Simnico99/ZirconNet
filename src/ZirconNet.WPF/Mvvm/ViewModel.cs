// <copyright file="ViewModel.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;

namespace ZirconNet.WPF.Mvvm;

public abstract class ViewModel : ObservableObject
{
    public void NotifyPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
