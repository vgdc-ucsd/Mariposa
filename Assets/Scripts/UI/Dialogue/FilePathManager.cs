using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class FilePathManager : Singleton<FilePathManager>
{
	// Paths and extensions to consider.
	[SerializeField] List<string> Paths;
	[SerializeField] List<string> Extensions;

	// Given a keyword, find it in one of the paths
	// Can return null.
	public string FindFullPath(string keyword)
	{
		// search for keyword within other files
		string wildcardedKeyword = "*" + keyword + "*";

		foreach(string path in Paths)
		{
			string[] searchResults = Directory.GetFiles(path, wildcardedKeyword); 
			if(searchResults == null)
			{
				continue;
			}

			foreach(string result in searchResults)
			{
				if(verifyAccepted(result))
				{
					return result;
				}
			}
		}

		return null;
	}

	// Given a path, check that it has an accepted extension.
	private bool verifyAccepted(string path)
	{
		foreach(string extension in Extensions)
		{
			if(path.Contains(extension))
			{
				return true;
			}
		}
		return false;
	}
}
