using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReportSystem.ViewModels;

namespace ReportSystem;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!;
        
        // 1. Try mapping ViewModel -> View (e.g. DashboardViewModel -> DashboardView)
        var viewName = name.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(viewName);

        // 2. Try mapping ViewModel -> Page (e.g. DashboardViewModel -> DashboardPage)
        if (type == null)
        {
            viewName = name.Replace("ViewModel", "Page", StringComparison.Ordinal);
            type = Type.GetType(viewName);
        }

        // 3. Try removing ViewModel suffix (e.g. DashboardViewModel -> Dashboard)
        if (type == null)
        {
            viewName = name.Replace("ViewModel", "", StringComparison.Ordinal);
            type = Type.GetType(viewName);
        }

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = param;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
    }
}