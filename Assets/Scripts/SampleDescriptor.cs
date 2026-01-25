using System;
using UnityEngine;

public sealed class SampleDescriptor
{
	public bool IsLabel { get; set; }

	public Type Type { get; set; }

	public string DisplayName { get; set; }

	public string Description { get; set; }

	public string CodeBlock { get; set; }

	public bool IsSelected { get; set; }

	public GameObject UnityObject { get; set; }

	public bool IsRunning
	{
		get
		{
			return UnityObject != null;
		}
	}

	public SampleDescriptor(Type type, string displayName, string description, string codeBlock)
	{
		Type = type;
		DisplayName = displayName;
		Description = description;
		CodeBlock = codeBlock;
	}

	public void CreateUnityObject()
	{
		if (!(UnityObject != null))
		{
			UnityObject = new GameObject(DisplayName);
			UnityObject.AddComponent(Type);
		}
	}

	public void DestroyUnityObject()
	{
		if (UnityObject != null)
		{
			UnityEngine.Object.Destroy(UnityObject);
			UnityObject = null;
		}
	}
}
