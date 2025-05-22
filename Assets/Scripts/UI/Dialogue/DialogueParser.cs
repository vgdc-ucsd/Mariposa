using UnityEngine;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;

public class DialogueException : Exception
{
	public DialogueException(string name, string message) 
		: base($"Error in dialogue {name}! {message}") { }
}

public class DialogueParser : Singleton<DialogueParser>
{
	private static IDeserializer deserializer = new DeserializerBuilder()
		.WithNamingConvention(PascalCaseNamingConvention.Instance)
		.IgnoreUnmatchedProperties()
		.Build();

	private static HashSet<string> portraits = new HashSet<string>
	{
		"MariposaNeutral", "MariposaSad", "MariposaSurprised", "MariposaHappy",
		"UnnamedNeutral",  "UnnamedSad", "UnnamedSurprised", "UnnamedSilhouette",
	};

	public static Dictionary<string, List<DialogueElement>> Parse(TextAsset yaml)
	{
		Dictionary<string, List<DialogueElement>> data = deserializer.Deserialize<Dictionary<string, List<DialogueElement>>>(yaml.text);
		return data;
	}

	public static void Validate(Dictionary<string, List<DialogueElement>> data)
	{
		foreach ((string name, List<DialogueElement> dialogue) in data)
		{
			bool first = true;
			foreach (DialogueElement element in dialogue)
			{
				if (first)
				{
					if (element.Speaker == null) throw new DialogueException(name, "The first entry does not contain the speaker's name!");
					first = false;
				}

				if (element.Line == null)
				{
					throw new DialogueException(name, "Empty line!");
				}

				// TODO
			}
		}
	}
}
