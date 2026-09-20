using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace CMkeyCombat
{
    static class CustomOverridesStore
    {
        const string FolderName = "CMkey";
        const string FileName = "custom-overrides.xml";

        public static string FilePath
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(Path.Combine(appData, FolderName), FileName);
            }
        }

        public static Dictionary<char, string> Load()
        {
            var result = new Dictionary<char, string>();
            string path = FilePath;
            if (!File.Exists(path)) return result;

            try
            {
                var document = new XmlDocument();
                document.Load(path);
                XmlNodeList nodes = document.SelectNodes("/cmkey-custom-overrides/mapping");
                if (nodes == null) return result;

                foreach (XmlNode node in nodes)
                {
                    XmlElement element = node as XmlElement;
                    if (element == null) continue;

                    int code;
                    if (!Int32.TryParse(element.GetAttribute("code"), NumberStyles.None,
                        CultureInfo.InvariantCulture, out code)
                        || code < Char.MinValue || code > Char.MaxValue)
                        continue;

                    string replacement = element.GetAttribute("replacement");
                    if (String.IsNullOrEmpty(replacement)) continue;
                    result[(char)code] = replacement;
                }
            }
            catch
            {
                // A malformed user file must not prevent CMkey from starting.
                result.Clear();
            }

            return result;
        }

        public static void Save(IDictionary<char, string> overrides)
        {
            string path = FilePath;
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string temporaryPath = path + ".tmp";
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                OmitXmlDeclaration = false
            };

            try
            {
                using (XmlWriter writer = XmlWriter.Create(temporaryPath, settings))
                {
                    writer.WriteStartElement("cmkey-custom-overrides");
                    writer.WriteAttributeString("version", "1");

                    var entries = new List<KeyValuePair<char, string>>(overrides);
                    entries.Sort((left, right) => left.Key.CompareTo(right.Key));
                    foreach (KeyValuePair<char, string> entry in entries)
                    {
                        if (String.IsNullOrEmpty(entry.Value)) continue;
                        writer.WriteStartElement("mapping");
                        writer.WriteAttributeString("code", ((int)entry.Key).ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("replacement", entry.Value);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(temporaryPath, path, null);
                    }
                    catch (PlatformNotSupportedException)
                    {
                        File.Delete(path);
                        File.Move(temporaryPath, path);
                    }
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    try { File.Delete(temporaryPath); } catch { }
                }
            }
        }
    }
}
