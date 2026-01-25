using System;

public class VersionContainer
{
	private enum EqualityVersion
	{
		Equally = 0,
		More = 1,
		Less = 2
	}

	public static readonly VersionContainer Zero = new VersionContainer();

	private readonly int[] _versionSource = new int[4];

	public int Production
	{
		get
		{
			return _versionSource[0];
		}
		set
		{
			_versionSource[0] = value;
		}
	}

	public int Major
	{
		get
		{
			return _versionSource[1];
		}
		set
		{
			_versionSource[1] = value;
		}
	}

	public int Minor
	{
		get
		{
			return _versionSource[2];
		}
		set
		{
			_versionSource[2] = value;
		}
	}

	public int DataVersion
	{
		get
		{
			return _versionSource[3];
		}
		set
		{
			_versionSource[3] = value;
		}
	}

	public VersionContainer()
	{
		SetVersion(0, 0, 0, 0);
	}

	public VersionContainer(string version)
	{
		SetVersion(version);
	}

	public VersionContainer(int production, int major, int minor, int dataVersion = -1)
	{
		SetVersion(production, major, minor, dataVersion);
	}

	public void SetVersion(string version)
	{
		version = version.Trim();
		if (string.IsNullOrEmpty(version))
		{
			SetVersion(0, 0, 0, 0);
			return;
		}
		string[] array = version.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
		SetVersion(Zero);
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				_versionSource[i] = int.Parse(array[i]);
			}
		}
		catch (Exception)
		{
			SetVersion(0, 0, 0, 0);
		}
	}

	public void SetVersion(VersionContainer version)
	{
		SetVersion(version.Production, version.Major, version.Minor, version.DataVersion);
	}

	public void SetVersion(int production, int major = -1, int minor = -1, int dataVersion = -1)
	{
		Production = production;
		Major = major;
		Minor = minor;
		DataVersion = dataVersion;
	}

	public static VersionContainer CreateVersion(string version)
	{
		return new VersionContainer(version);
	}

	public static VersionContainer CreateVersion(VersionContainer version)
	{
		return new VersionContainer(version.Production, version.Major, version.Minor, version.DataVersion);
	}

	public static VersionContainer CreateVersion(int production, int major = -1, int minor = -1, int dataVersion = -1)
	{
		return new VersionContainer(production, major, minor, dataVersion);
	}

	public string ToString(bool isDataVersion)
	{
		if (isDataVersion)
		{
			return string.Format("{0}.{1}.{2}.{3}", Production, Major, Minor, DataVersion);
		}
		return string.Format("{0}.{1}.{2}", Production, Major, Minor);
	}

	public override bool Equals(object obj)
	{
		return obj is VersionContainer && (VersionContainer)obj == this;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return ToString(false);
	}

	public bool Empty(bool isDataVersion = false)
	{
		if (Production == 0 && Major == 0 && Minor == 0 && (!isDataVersion || DataVersion == 0))
		{
			return true;
		}
		return false;
	}

	private static EqualityVersion Compare(VersionContainer a, VersionContainer b, int dimensions = 4)
	{
		int[] versionSource = a._versionSource;
		int[] versionSource2 = b._versionSource;
		for (int i = 0; i < dimensions; i++)
		{
			if (versionSource[i] != versionSource2[i])
			{
				return (versionSource[i] > versionSource2[i]) ? EqualityVersion.More : EqualityVersion.Less;
			}
		}
		return EqualityVersion.Equally;
	}

	public bool ForCurrentVersion(string dataVersion)
	{
		return ForCurrentVersion(new VersionContainer(dataVersion));
	}

	public bool ForCurrentVersion(VersionContainer dataVersion)
	{
		return Compare(this, dataVersion, 3) == EqualityVersion.Equally;
	}

	public bool ForNextVersions(VersionContainer dataVersion)
	{
		return Compare(this, dataVersion, 3) == EqualityVersion.Less;
	}

	public static bool operator ==(VersionContainer a, VersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator ==(VersionContainer a, string b)
	{
		return a == CreateVersion(b);
	}

	public static bool operator !=(VersionContainer a, VersionContainer b)
	{
		return !(a == b);
	}

	public static bool operator !=(VersionContainer a, string b)
	{
		return a != CreateVersion(b);
	}

	public static bool operator >(VersionContainer a, VersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.More && equalityVersion != EqualityVersion.Equally;
	}

	public static bool operator >(VersionContainer a, string b)
	{
		return a > CreateVersion(b);
	}

	public static bool operator <(VersionContainer a, VersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Less && equalityVersion != EqualityVersion.Equally;
	}

	public static bool operator <(VersionContainer a, string b)
	{
		return a < CreateVersion(b);
	}

	public static bool operator >=(VersionContainer a, VersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.More || equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator >=(VersionContainer a, string b)
	{
		return a >= CreateVersion(b);
	}

	public static bool operator <=(VersionContainer a, VersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Less || equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator <=(VersionContainer a, string b)
	{
		return a <= CreateVersion(b);
	}

	public static implicit operator string(VersionContainer v)
	{
		return v.ToString(true);
	}
}
