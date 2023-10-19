using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class Deploy : MonoBehaviour 
{
	static bool allFilesExists;
	[MenuItem ("SmartFoxServer 2X/Examples deployment")]
	static void DeployExamples () 
	{
		
		if(EditorUtility.DisplayDialog("Examples deployment", 
		                               "Some examples require a server side Extension containing the game logic to be deployed on the server.\n" +
		                               "Click the \"Deploy\" button and select SmartFoxServer 2X instalation folder when prompted.\n" +
		                               "Start (or restart) the server when deployment is done.", "Deploy", "Cancel"))
		{
			string path = EditorUtility.OpenFolderPanel("Select SmartFoxServer 2X's installation folder", "", "");
			if(path != "")
			{
				string[] folders = Directory.GetDirectories(path);
				foreach(string folder in folders)
				{
					if(folder == path + Path.DirectorySeparatorChar + "SFS2X")
					{
						path = folder;
						break;
					}
				}
				
				folders = Directory.GetDirectories(path);
				bool zonesExists = false;
				bool extensionsExists = false;
				foreach(string folder in folders)
				{
					if(folder == path + Path.DirectorySeparatorChar + "extensions")
						extensionsExists = true;
					if(folder == path + Path.DirectorySeparatorChar + "zones")
						zonesExists = true;
				}
				
				if(extensionsExists && zonesExists)
				{
					allFilesExists = true;
					PerformDeepCopy(Application.dataPath + Path.DirectorySeparatorChar + "Deploy", path);
					if(allFilesExists)
					{
						EditorUtility.DisplayDialog("Extensions deployed!", "Please start (or restart) SFS2X server", "Ok");
					}
					else
					{
						EditorUtility.DisplayDialog("Deployment unsuccessfull", "There was a problem while deploying the extensions on SmartFoxServer 2X's folder.\n" +
						                            "Please copy the extensions manually from the Deploy/ folder to /SFS2X/extensions/.", "Ok");
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Wrong path", "Please make sure to select SmartFoxServer 2X installation folder.", "Close");
				}
			}
		}
	}
	
	static void PerformDeepCopy(string sourceDirectory, string destinationDirectory)
	{
		if (!Directory.Exists(destinationDirectory))
		{
			Directory.CreateDirectory(destinationDirectory);
		}
		
		var sourceDir = new DirectoryInfo(sourceDirectory);
		var targetDir = new DirectoryInfo(destinationDirectory);
		
		foreach (FileInfo f in sourceDir.GetFiles())
		{
			if(f.Extension != ".meta")
			{
				String file = Path.Combine(targetDir.ToString(), f.Name);
				try
				{
					f.CopyTo(file, true);
				}
				catch(Exception e)
				{
					Debug.LogError(e.Message);
					allFilesExists = false;
				}
				
				if(!File.Exists(file))
				{
					allFilesExists = false;
				}
			}
		}
		
		foreach (DirectoryInfo dir in sourceDir.GetDirectories())
		{
			var subDirectory = targetDir.CreateSubdirectory(dir.Name);
			PerformDeepCopy(dir.FullName, subDirectory.FullName);
		}
	}
}
