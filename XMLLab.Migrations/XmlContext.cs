using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XMLLab.Migrations {
    public class XmlContext {
        readonly string versionAttribute;
        private readonly Encoding encoding;
        readonly string xml;
        internal XDocument Document { get; }
        public Version Version { get; private set; }
        public XmlContext(string xml, string versionAttribute, Encoding encoding) {
            this.xml = xml;
            this.versionAttribute = versionAttribute;
            this.encoding = encoding;
            using (var str = new MemoryStream(encoding.GetBytes(xml))) {
                Document = XDocument.Load(str, LoadOptions.None);
            }

            var ver = Document.Root.Attributes(versionAttribute).SingleOrDefault();
            if (ver == null) {
                Version = new Version(0, 0, 0, 0);
                Document.Root.SetAttributeValue(versionAttribute, Version.ToString());
            } else {
                Version = new Version(ver.Value);
            }
        }
        public XmlContext(string xml) :this(xml, "version", Encoding.UTF8) {
           
        }

        public string SerializeText() {
            using (var str = new MemoryStream()) {
                Document.Save(str, SaveOptions.None);

                return encoding.GetString(str.ToArray());
            }
        }
        public Stream Serialize() {
            var str = new MemoryStream();
            Document.Save(str);
            str.Position = 0;
            return str;
        }

        public void SetVersion(Version to) {
            Version = to;
            Document.Root.SetAttributeValue(versionAttribute, Version.ToString());
        }

        public T DeserializeAs<T>() {
            var s = new XmlSerializer(typeof(T));
            return(T)s.Deserialize(Serialize());
        }
    }
}
