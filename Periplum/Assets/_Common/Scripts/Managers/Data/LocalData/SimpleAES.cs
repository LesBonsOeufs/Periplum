using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

public static class SimpleAES
{
	#region Cipher

	public static string Cipher(string pTextToCipher, string pKey)
	{
		return Cipher(ObjectToByteArray(pTextToCipher), pKey);
	}

	public static string Cipher(byte[] pArrayToCipher, string pKey)
	{
		byte[] lKeyArray = Encoding.UTF8.GetBytes(pKey);

		RijndaelManaged lAES = new RijndaelManaged
		{
			Key = lKeyArray,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		};

		ICryptoTransform lTransform = lAES.CreateEncryptor();
		byte[] resultArray = lTransform.TransformFinalBlock(pArrayToCipher, 0, pArrayToCipher.Length);

		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	#endregion

	#region Decipher

	public static byte[] Decipher(string pTextToDecipher, string pKey)
	{
		byte[] lKeyArray = Encoding.UTF8.GetBytes(pKey);
		byte[] lToEncryptArray = Convert.FromBase64String(pTextToDecipher);

		RijndaelManaged lAES = new RijndaelManaged
		{
			Key = lKeyArray,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		};

		ICryptoTransform lTransform = lAES.CreateDecryptor();

		return lTransform.TransformFinalBlock(lToEncryptArray, 0, lToEncryptArray.Length);
	}

	public static T DecipherToObject<T>(string pTextToDecipher, string pKey)
	{
		return (T)ByteArrayToObject(Decipher(pTextToDecipher, pKey));
	}

	#endregion

	#region Object to Bytes conversions

	public static byte[] ObjectToByteArray(object pObject)
	{
		if (pObject == null)
			return null;

		BinaryFormatter lBinFormatter = new BinaryFormatter();
		MemoryStream lMemStream = new MemoryStream();

		lBinFormatter.Serialize(lMemStream, pObject);

		return lMemStream.ToArray();
	}

	public static object ByteArrayToObject(byte[] pByteArray)
	{
		MemoryStream lMemStream = new MemoryStream();
		BinaryFormatter lBinFormatter = new BinaryFormatter();

		lMemStream.Write(pByteArray, 0, pByteArray.Length);
		lMemStream.Seek(0, SeekOrigin.Begin);

		object lObject = lBinFormatter.Deserialize(lMemStream);

		return lObject;
	}

	#endregion
}