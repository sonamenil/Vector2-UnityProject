using System;
using System.Collections.Generic;
using System.IO;
using DiffMatchPatch;
using Nekki.Vector.Core;
using UnityEngine;

public static class PatchUtility
{
	private static diff_match_patch _Diff = new diff_match_patch();

	public static void CreatePatch(string p_fileA, string p_fileB, string p_output)
	{
		try
		{
			string text = File.ReadAllText(p_fileA);
			string text2 = File.ReadAllText(p_fileB);
			List<Patch> patches = _Diff.patch_make(text, text2);
			string text3 = _Diff.patch_toText(patches);
			if (!string.IsNullOrEmpty(text3))
			{
				FileUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(p_output));
				File.WriteAllText(p_output, text3);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void ApplyPatch(string p_patch, string p_file, string p_output = null)
	{
		try
		{
			string textline = File.ReadAllText(p_patch);
			string text = File.ReadAllText(p_file);
			List<Patch> patches = _Diff.patch_fromText(textline);
			object[] array = _Diff.patch_apply(patches, text);
			if (p_output == null)
			{
				p_output = p_file;
			}
			FileUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(p_output));
			File.WriteAllText(p_output, (string)array[0]);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void ApplyPatchToText(string p_patch, string p_text, string p_output)
	{
		try
		{
			string textline = File.ReadAllText(p_patch);
			List<Patch> patches = _Diff.patch_fromText(textline);
			object[] array = _Diff.patch_apply(patches, p_text);
			FileUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(p_output));
			File.WriteAllText(p_output, (string)array[0]);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}
