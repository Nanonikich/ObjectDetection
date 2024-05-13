using System;

using Avalonia.Controls.Templates;
using Avalonia.Controls;

using ObjectDetection.Gui.Common.ViewModels;

namespace ObjectDetection.Gui;

public class ViewLocator : IDataTemplate
{
	public Control Build(object data)
	{
		var name = data.GetType().FullName!.Replace("ViewModel", "View");
		var type = Type.GetType(name);

		if(type != null)
		{
			return (Control)Activator.CreateInstance(type)!;
		}
		else
		{
			return new TextBlock { Text = "Not Found: " + name };
		}
	}

	public bool Match(object data)
	{
		return data is ViewModelBase;
	}
}
