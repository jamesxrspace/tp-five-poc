using System.Text;
using System.Xml;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// Anroid xml document.
    /// </summary>
    public class AndroidXmlDocument : XmlDocument
    {
        /// <summary>
        /// The uri of android xml namespace.
        /// </summary>
        public static readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

        /// <summary>
        /// The uri of tools xml namespace.
        /// </summary>
        public static readonly string ToolsXmlNamespace = "http://schemas.android.com/tools";
        private readonly XmlNamespaceManager namespaceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidXmlDocument"/> class.
        /// </summary>
        /// <param name="fileName">The fileName of document.</param>
        public AndroidXmlDocument(string fileName)
        {
            this.FileName = fileName;
            using (var reader = new XmlTextReader(this.FileName))
            {
                reader.Read();
                this.Load(reader);
            }

            namespaceManager = new XmlNamespaceManager(NameTable);
            namespaceManager.AddNamespace("android", AndroidXmlNamespace);
        }

        /// <summary>
        /// Gets the fileName of document.
        /// </summary>
        /// <value>A string that indicates the name of the file.</value>
        public string FileName { get; private set; }

        /// <summary>
        /// Save the document.
        /// </summary>
        public void Save()
        {
            SaveAs(FileName);
        }

        /// <summary>
        /// Save the document to the specified path.
        /// </summary>
        /// <param name="path">A string that indicates the name of the file to be save.</param>
        public void SaveAs(string path)
        {
            using var writer = new XmlTextWriter(path, new UTF8Encoding(false));
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
    }
}
