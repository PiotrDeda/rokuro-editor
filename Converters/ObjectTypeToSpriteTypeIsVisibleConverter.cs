using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RokuroEditor.Models;

namespace RokuroEditor.Converters;

public class ObjectTypeToSpriteTypeIsVisibleConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is GameObjectType selectedGameObjectType
			? selectedGameObjectType.Name != "Rokuro.Objects.TextObject"
			: true;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotImplementedException();
}
