using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class FilePathManager : Singleton<FilePathManager>
{
	[SerializeField] List<string> Paths;
	[SerializeField] List<string> Extensions;

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
