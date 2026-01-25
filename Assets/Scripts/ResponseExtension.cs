using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public static class ResponseExtension
{
	public static void WriteString(this HttpListenerResponse response, string input, string type = "text/plain")
	{
		response.StatusCode = 200;
		response.StatusDescription = "OK";
		if (!string.IsNullOrEmpty(input))
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			response.ContentLength64 = bytes.Length;
			response.ContentType = type;
			response.OutputStream.Write(bytes, 0, bytes.Length);
		}
	}

	public static void WriteBytes(this HttpListenerResponse response, byte[] bytes)
	{
		response.StatusCode = 200;
		response.StatusDescription = "OK";
		response.ContentLength64 = bytes.Length;
		response.OutputStream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteFile(this HttpListenerResponse response, string path, string type = "application/octet-stream", bool download = false)
	{
		using (FileStream fileStream = File.OpenRead(path))
		{
			response.StatusCode = 200;
			response.StatusDescription = "OK";
			response.ContentLength64 = fileStream.Length;
			response.ContentType = type;
			if (download)
			{
				response.AddHeader("Content-disposition", string.Format("attachment; filename={0}", Path.GetFileName(path)));
			}
			byte[] array = new byte[65536];
			int count;
			while ((count = fileStream.Read(array, 0, array.Length)) > 0)
			{
				Thread.Sleep(0);
				response.OutputStream.Write(array, 0, count);
			}
		}
	}
}
