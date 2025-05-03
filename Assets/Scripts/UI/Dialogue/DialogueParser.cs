using UnityEngine;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class DialogueOptionIntermediate
{
	public string text;
	public List<string> impact;
	public List<DialogueElementIntermediate> script;

	public override string ToString() 
	{
		string output = "Option titled \"" + text + "\"\n";
		output += "Impact: ";
		foreach (string aspect in impact)
		{
			output += aspect + " ";
		}
		output += "Script:\n";
		if(script != null)
		{
			foreach (DialogueElementIntermediate ele in script) 
			{
				if(ele != null)
					output += ele.ToString();
			}
		}
		else
		{
			output += "No script.\n";
		}
		return output;
	}

}

public class DialogueElementIntermediate 
{
	public string name;
	public string line;
	public string sound;
	public string icon;
	public List<DialogueOptionIntermediate> options;

	public override string ToString() 
	{
		string output = name + ": [" + line + "] <plays: " + sound + " looks: " + icon + ">\n";
		if(options == null)
		{
			output += "No options.\n";
		}
		else if(options.Count == 0)
		{
			output += "No options.\n";
		}
		else 
		{
			output += options.Count + " option";
			if(options.Count == 1)
			{
				output += ":\n";
			}
			else 
			{
				output += "s:\n";
			}
			foreach (var opt in options) 
			{
				if(opt != null)
				{
					output += opt.ToString();
				}
				else 
				{
					output += "Invalid option\n";
				}
			}
		}
		return output;
	}
}

public class DialogueParser 
{
	private static IDeserializer deserializer = new DeserializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.IgnoreUnmatchedProperties()
		.Build();

	private string ArtPath;
	private string YamlPath;

	public DialogueParser(string ArtPath, string YamlPath)
	{
		this.ArtPath = ArtPath;
		this.YamlPath = YamlPath;
	}
	public Dialogue ParseDialogue(string path)
	{
		/*FileStream file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);*/
		string yml = File.ReadAllText(path);
		var inter = deserializer.Deserialize<List<DialogueElementIntermediate>>(yml);
		Dialogue parsedDialogue = new Dialogue();
		parsedDialogue.Conversation = new List<DialogueElement>();
		foreach (DialogueElementIntermediate ele in inter) 
		{
			DialogueElement realEle = new DialogueElement();
			realEle.Speaker = ele.name;
			realEle.Line = ele.line;
			realEle.FromRadio = false; // oversight: did not implement this
			realEle.Sprite = Resources.Load(ArtPath + ele.icon) as Sprite;

			// the rest of the fields are actually lost for now.

			parsedDialogue.Conversation.Add(realEle);
		}
		return parsedDialogue;
	}
	static List<DialogueElementIntermediate> ParseDialogueTest(string path)
	{
		string yml = File.ReadAllText(path);

		var p = deserializer.Deserialize<List<DialogueElementIntermediate>>(yml);

		return p;
		/*return returnable;*/
	}

	public static void SelfTest(string path)
	{
		var test = ParseDialogueTest(path);

		Debug.Log("Length: " + test.Count.ToString());

		foreach (DialogueElementIntermediate ele in test)
		{
			Debug.Log(ele.ToString());
		}
	}
}
