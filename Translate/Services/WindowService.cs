using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Translate.ViewModels;
using Translate.Views;

namespace Translate.Services;

/// <summary>
/// Implementation of IWindowService using ViewModel-to-Window mapping.
/// </summary>
public class WindowService : IWindowService
{
    // ViewModel type -> Window factory mapping
    private readonly Dictionary<Type, Func<Window>> _windowFactories = new()
    {
        { typeof(SettingViewModel), () => new SettingWindow() }
        // Add new mappings here: { typeof(AboutViewModel), () => new AboutWindow() }
    };

    public void ShowWindow<TViewModel>() where TViewModel : ObservableObject
    {
        var viewModelType = typeof(TViewModel);

        if (_windowFactories.TryGetValue(viewModelType, out var factory))
        {
            var window = factory();
            window.Activate();
        }
        else
        {
            throw new InvalidOperationException($"No window registered for ViewModel type: {viewModelType.Name}");
        }
    }
}
