using System;
using System.IO;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using ReactiveUI;
using RokuroEditor.Models;
using RokuroEditor.ViewModels;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RokuroEditor.Views.Panels;

public partial class GamePreviewPanel : ReactiveUserControl<GamePreviewViewModel>
{
	public GamePreviewPanel()
	{
		InitializeComponent();
		this.WhenActivated(d => {
			d(ViewModel!.ProjectData.Changed.Subscribe(new Action<object>(_ => RenderGameObjects())));
		});
		// I absolutely hate this but I haven't found a better way
		this.WhenActivated(d => {
			this.WhenAnyValue(
					x => x.ViewModel!.ProjectData.SelectedGameObject!.SpriteType,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.Camera,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.PositionX,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.PositionY,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.ScaleX,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.ScaleY,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.Rotation
				)
				.Subscribe(_ => RenderGameObjects())
				.DisposeWith(d);
		});
		this.WhenActivated(d => {
			this.WhenAnyValue(
					x => x.ViewModel!.ProjectData.SelectedGameObject!.FlipX,
					x => x.ViewModel!.ProjectData.SelectedGameObject!.FlipY,
					x => x.ViewModel!.ProjectData.SelectedCamera!.Name
				)
				.Subscribe(_ => RenderGameObjects())
				.DisposeWith(d);
		});
	}

	public void RenderGameObjects()
	{
		PreviewCanvas.Children.Clear();
		foreach (GameObject gameObject in ViewModel?.ProjectData.SelectedScene?.GameObjects ?? new())
			if (gameObject.Class.Name == "Rokuro.Objects.TextObject")
			{
				var textControl = new TextBlock {
					Text = gameObject.Sprite,
					FontSize = 16,
					Foreground = Brushes.White,
					Margin = new(gameObject.PositionX, gameObject.PositionY, 0, 0)
				};
				PreviewCanvas.Children.Add(textControl);
			}
			else
			{
				Bitmap image;
				TextureConfigModel textureConfig = new();
				try
				{
					image = new(Path.Combine(ViewModel!.ProjectData.ProjectPath!, "assets", "textures",
						gameObject.Sprite) + ".png");

					string configFilename = Path.Combine(ViewModel!.ProjectData.ProjectPath!, "assets", "textures",
						gameObject.Sprite) + ".yaml";
					if (File.Exists(configFilename))
						textureConfig = new DeserializerBuilder()
							.WithNamingConvention(UnderscoredNamingConvention.Instance)
							.IgnoreUnmatchedProperties()
							.Build()
							.Deserialize<TextureConfigModel>(File.ReadAllText(configFilename));
				}
				catch (Exception)
				{
					continue;
				}
				CroppedBitmap cropped = new(image, new(0, 0,
					image.PixelSize.Width / textureConfig.Frames, image.PixelSize.Height / textureConfig.States));
				TransformGroup transformGroup = new() {
					Children = {
						new ScaleTransform(gameObject.ScaleX * (gameObject.FlipX ? -1 : 1),
							gameObject.ScaleY * (gameObject.FlipY ? -1 : 1)),
						new RotateTransform(gameObject.Rotation),
						new TranslateTransform(gameObject.PositionX, gameObject.PositionY)
					}
				};
				var imageControl = new Image {
					Source = cropped,
					Width = cropped.Size.Width,
					Height = cropped.Size.Height,
					RenderTransform = transformGroup
				};
				PreviewCanvas.Children.Add(imageControl);
			}
	}

	class TextureConfigModel
	{
		[UsedImplicitly] public int States { get; set; } = 1;
		[UsedImplicitly] public int Frames { get; set; } = 1;
		[UsedImplicitly] public int Delay { get; set; } = 30;
	}
}
