using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TraPutty
{
	public class ApplicationSetting
	{
		public byte alpha = 210;
		public bool puttysAreTransparent = false;
		public bool notTransparentPuttySetting = false;
		public List<string> ngwords;

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public static ApplicationSetting Load() {
            try {
                var serializer = new XmlSerializer(typeof(ApplicationSetting));
                var stream = new FileStream("TraPutty.xml", FileMode.Open);
                return (ApplicationSetting)serializer.Deserialize(stream);
            }
            catch {
                var settings = new ApplicationSetting();
                settings.ngwords = new List<string>() { "sidebar", "Saezuri" };
                return settings;
            }
        }

        /// <summary>
        /// 設定を保存
        /// </summary>
        public void Save() {
            try {
                var sirializer = new XmlSerializer(typeof(ApplicationSetting));
                var stream = new FileStream("TraPutty.xml", FileMode.Create);
                sirializer.Serialize(stream, this);
                stream.Close();
            }
            catch { }
        }
	}
}
