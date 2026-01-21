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

    // Track open windows to implement singleton pattern
    private readonly Dictionary<Type, Window> _openWindows = new();

    public void ShowWindow<TViewModel>() where TViewModel : ObservableObject
    {
        var viewModelType = typeof(TViewModel);

        // Check if window is already open
        if (_openWindows.TryGetValue(viewModelType, out var existingWindow))
        {
            // Activate the existing window
            existingWindow.Activate();
            return;
        }

        if (_windowFactories.TryGetValue(viewModelType, out var factory))
        {
            var window = factory();

            // Track the window and remove it when closed
            _openWindows[viewModelType] = window;
            window.Closed += (s, e) => _openWindows.Remove(viewModelType);

            window.Activate();
        }
        else
        {
            throw new InvalidOperationException($"No window registered for ViewModel type: {viewModelType.Name}");
        }
    }
}
