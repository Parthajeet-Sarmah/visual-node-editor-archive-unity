using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseUtility
{
	public const char UnityDirectorySeperator = '/';
	public const string ResourcesFolderName = "Resources";

	public static void CreateAssetsAndDirectories(Object unityObject, string unityFilePath)
	{
		var pathDirectory = Path.GetDirectoryName (unityFilePath) + UnityDirectorySeperator;
		CreateDirectoriesInPath (pathDirectory);
		AssetDatabase.CreateAsset (unityObject, unityFilePath);
	}

	private static void CreateDirectoriesInPath(string unityDirectoryPath)
	{
		if(unityDirectoryPath[unityDirectoryPath.Length - 1] != UnityDirectorySeperator)
		{
			string warning = string.Format ("Path supplied to CreateDirectoriesInPath does not include UnityDirectorySeparator as last char." +
			                 "\nSupplied path: {0}", unityDirectoryPath);

			Debug.LogWarning (warning);
		}

		var filename = Path.GetFileName (unityDirectoryPath);

		if(!string.IsNullOrEmpty(filename))
		{
			var warningMessage = string.Format ("Path supplied to CreateDirectoriesInPath contains a file name. This will be stripped." +
			                     "\nSupplied path: {0}, File name: {1}", unityDirectoryPath, filename);

			Debug.LogWarning (warningMessage);
			unityDirectoryPath = unityDirectoryPath.Replace (filename, string.Empty);
		}

		var folders = unityDirectoryPath.Split (UnityDirectorySeperator);

		if(folders.Length > 0 && folders[0] != "Assets")
		{
			var exception = "AssetDatabaseUtility CreateDirectoriesInPath expects full Unity path, including 'Assets\\\"." + "Adding Assets to path";

			throw new UnityException (exception);
		}

		string pathToFolder = string.Empty;
		foreach (var folder in folders) 
		{
			if(string.IsNullOrEmpty(folder))
			{
				continue;
			}

			pathToFolder = string.Concat (pathToFolder, folder);

			if(!AssetDatabase.IsValidFolder(pathToFolder))
			{
				var pathToParent = System.IO.Directory.GetParent (pathToFolder).ToString ();
				AssetDatabase.CreateFolder (pathToParent, folder);
				AssetDatabase.Refresh ();
			}

			pathToFolder = string.Concat (pathToFolder, UnityDirectorySeperator);
		}
	}
}

