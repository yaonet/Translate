using CommunityToolkit.Mvvm.ComponentModel;

namespace Translate.Services;

/// <summary>
/// Service for managing application windows using ViewModel-based navigation.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Opens a window associated with the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to.</typeparam>
    void ShowWindow<TViewModel>() where TViewModel : ObservableObject;
}
