using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RokuroEditor.Models;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class GamePreviewPanel : ReactiveUserControl<GamePreviewViewModel>
{
	public GamePreviewPanel()
	{
		InitializeComponent();
		this.WhenActivated(d => {
			d(ViewModel!.ProjectData.Changed.Subscribe(new Action<object>(_ => RenderGameObjects())));
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
				try
				{
					image = new(Path.Combine(ViewModel!.ProjectData.ProjectPath!, "assets", "textures",
						gameObject.Sprite) + ".png");
				}
				catch (FileNotFoundException)
				{
					continue;
				}
				var imageControl = new Image {
					Source = image,
					Width = image.Size.Width,
					Height = image.Size.Height,
					Margin = new(gameObject.PositionX, gameObject.PositionY, 0, 0)
				};
				PreviewCanvas.Children.Add(imageControl);
			}
	}
}
