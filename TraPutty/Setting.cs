using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraPutty
{
	public class ApplicationSetting
	{
		[System.Xml.Serialization.XmlElement("alpha")]
		public byte alphaValue = 210;
		public bool puttysAreTransparent = false;
		public bool notTransparentPuttySetting = false;
		public string[] ngwords = new string[] { "sidebar", "Saezuri" };
	}
}
