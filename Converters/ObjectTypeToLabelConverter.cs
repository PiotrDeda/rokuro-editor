using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RokuroEditor.Models;

namespace RokuroEditor.Converters;

public class ObjectTypeToLabelConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is GameObjectType selectedGameObjectType
			? selectedGameObjectType.Name == "Rokuro.Objects.TextObject" ? "Text" : "Sprite"
			: "Sprite";

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
