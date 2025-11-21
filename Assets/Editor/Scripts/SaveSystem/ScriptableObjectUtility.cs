using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
	public static T LoadSaveData<T> (string unityPathFile) where T : ScriptableObject
	{
		string resourcesFolder = string.Concat ("/", "Resources", "/");

		if(!unityPathFile.Contains(resourcesFolder))
		{
			string exception = string.Format ("Failed to load scriptable object of type , {0}, from path : {1}. ", typeof(T).ToString (), unityPathFile);

			throw new UnityException (exception);
		}

		string resourceRelativePath = GetResourceRelativeString (unityPathFile);

		string fileExtension = Path.GetExtension (unityPathFile);
		resourceRelativePath = resourceRelativePath.Replace (fileExtension, string.Empty);

		return Resources.Load<T> (resourceRelativePath);
	}

	public static T LoadOrCreateSaveData<T> (string unityPathToFile) where T : ScriptableObject
	{
		var loadedSettings = LoadSaveData<T> (unityPathToFile);

		if(loadedSettings == null)
		{
			loadedSettings = ScriptableObject.CreateInstance<T> ();
			AssetDatabaseUtility.CreateAssetsAndDirectories (loadedSettings, unityPathToFile);
		}

		return loadedSettings;
	}

	private static void DeleteSaveData(string fullUnityPath)
	{
		File.Delete (fullUnityPath);
	}

	private static string GetResourceRelativeString(string unityPath)
	{
		string resourcesFolder = "Resources/";
		string pathToResources = unityPath.Substring (0, unityPath.IndexOf (resourcesFolder));

		pathToResources = unityPath.Replace (pathToResources, string.Empty);

		pathToResources = pathToResources.Replace (resourcesFolder, string.Empty);

		return pathToResources;
	}
}

