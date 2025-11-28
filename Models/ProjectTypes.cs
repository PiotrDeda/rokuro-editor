using System.Collections.ObjectModel;
using ReactiveUI;

namespace RokuroEditor.Models;

public class ProjectTypes(
	ObservableCollection<GameObjectType> gameObjectTypes,
	ObservableCollection<CameraType> cameraTypes,
	ObservableCollection<SpriteType> spriteTypes,
	ObservableCollection<SceneType> sceneTypes
) : ReactiveObject
{
	public ProjectTypes() : this([], [], [], []) {}

	public ObservableCollection<GameObjectType> GameObjectTypes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = gameObjectTypes;

	public ObservableCollection<CameraType> CameraTypes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = cameraTypes;

	public ObservableCollection<SpriteType> SpriteTypes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = spriteTypes;

	public ObservableCollection<SceneType> SceneTypes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = sceneTypes;
}
