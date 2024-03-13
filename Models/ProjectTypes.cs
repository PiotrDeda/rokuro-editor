using System.Collections.ObjectModel;
using ReactiveUI;

namespace RokuroEditor.Models;

public class ProjectTypes(
	ObservableCollection<GameObjectType> gameObjectTypes,
	ObservableCollection<CameraType> cameraTypes,
	ObservableCollection<SpriteType> spriteTypes) : ReactiveObject
{
	ObservableCollection<CameraType> _cameraTypes = cameraTypes;
	ObservableCollection<GameObjectType> _gameObjectTypes = gameObjectTypes;
	ObservableCollection<SpriteType> _spriteTypes = spriteTypes;

	public ProjectTypes() : this(new(), new(), new()) {}

	public ObservableCollection<GameObjectType> GameObjectTypes
	{
		get => _gameObjectTypes;
		set => this.RaiseAndSetIfChanged(ref _gameObjectTypes, value);
	}

	public ObservableCollection<CameraType> CameraTypes
	{
		get => _cameraTypes;
		set => this.RaiseAndSetIfChanged(ref _cameraTypes, value);
	}

	public ObservableCollection<SpriteType> SpriteTypes
	{
		get => _spriteTypes;
		set => this.RaiseAndSetIfChanged(ref _spriteTypes, value);
	}
}
