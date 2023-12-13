using System.Collections.Generic;
using System.Linq;

namespace Oirenomi.Models;

public class ProjectData
{
	public string? ProjectPath { get; set; }
	public string? ProjectName { get; set; }
	public List<Scene> Scenes { get; set; } = new();

	public void SetProjectPathAndName(string? projectPath)
	{
		ProjectPath = projectPath;
		ProjectName = ProjectPath?.Split('\\').Last().Split('.').First();
	}

	public bool BuildProject()
	{
		if (ProjectPath is null || ProjectName is null)
			return false;
		ProjectBuilder.Build(ProjectPath, ProjectName);
		return true;
	}

	public bool LoadProject()
	{
		if (ProjectPath is null || ProjectName is null)
			return false;

		BuildProject();

		return true;
	}
}
