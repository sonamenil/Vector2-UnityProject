using System.Xml;

public class DeviceComparison : ComparisonExpression
{
	public enum DeviceParameter
	{
		PARAMETER_NONE = 0,
		MEMORY_TOTAL = 1,
		MEMORY_FREE = 2,
		CORES_COUNT = 3
	}

	private DeviceParameter _parameter;

	public DeviceComparison(XmlNode node)
		: base(node)
	{
		if (node.Attributes != null)
		{
			_parameter = GetParameterFromString((node.Attributes["Value"] != null) ? node.Attributes["Value"].Value : null);
			_second = float.Parse(node.Attributes["Than"].Value);
		}
		UpdateParameter();
	}

	public static DeviceParameter GetParameterFromString(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			AdvLog.LogError("DeviceComparison::GetParameterFromString - empty parameter name");
			return DeviceParameter.PARAMETER_NONE;
		}
		switch (name)
		{
		case "_DeviceTotalMem":
			return DeviceParameter.MEMORY_TOTAL;
		case "_DeviceFreeMem":
			return DeviceParameter.MEMORY_FREE;
		case "_DeviceCoresNum":
			return DeviceParameter.CORES_COUNT;
		default:
			AdvLog.LogError(string.Format("DeviceComparison::GetParameterFromString - unknown type: {0}", name));
			return DeviceParameter.PARAMETER_NONE;
		}
	}

	public void UpdateParameter()
	{
		switch (_parameter)
		{
		case DeviceParameter.MEMORY_TOTAL:
			_first = SystemProperties.SystemMemory / 1024f;
			break;
		case DeviceParameter.MEMORY_FREE:
			_first = SystemProperties.FreeMemory / 1024f;
			break;
		case DeviceParameter.CORES_COUNT:
			_first = SystemProperties.CoreCount;
			break;
		default:
			AdvLog.LogError(string.Format("DeviceComparison::UpdateParameter - unknown type: {0}", _parameter));
			break;
		}
	}
}
