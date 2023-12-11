using System.Xml;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// An android manifest class to mainpulate xml elements.
    /// </summary>
    public class AndroidManifest : AndroidXmlDocument
    {
        private readonly XmlElement applicationElement;
        private readonly XmlElement manifestElement;
        private readonly XmlNamespaceManager namespaceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidManifest"/> class.
        /// </summary>
        /// <param name="path">The path of AndroidManifest.xml.</param>
        public AndroidManifest(string path)
            : base(path)
        {
            namespaceManager = new XmlNamespaceManager(NameTable);
            namespaceManager.AddNamespace("android", AndroidXmlNamespace);
            namespaceManager.AddNamespace("tools", ToolsXmlNamespace);

            manifestElement = SelectSingleNode("/manifest") as XmlElement;
            applicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        }

        /// <summary>
        /// Add or update meta-data into applicaion.
        /// </summary>
        /// <param name="name">The name of meta-data.</param>
        /// <param name="value">The value of meta-data.</param>
        /// <param name="toolsNodeValue">The value of tools node.</param>
        public void SetApplicationMetaData(string name, string value, string toolsNodeValue = null)
        {
            var xpath = $"meta-data[@android:name={name}]";
            var element = (XmlElement)applicationElement.SelectSingleNode(xpath, namespaceManager);
            if (element == null)
            {
                element = CreateElement("meta-data");
                element.SetAttribute("name", AndroidXmlNamespace, name);
                applicationElement.AppendChild(element);
            }

            element.SetAttribute("value", AndroidXmlNamespace, value);
            if (!string.IsNullOrEmpty(toolsNodeValue))
            {
                _ = element.SetAttribute("node", ToolsXmlNamespace, toolsNodeValue);
            }
            else
            {
                element.RemoveAttribute("node", ToolsXmlNamespace);
            }
        }

        /// <summary>
        /// Add or update uses-permission.
        /// </summary>
        /// <param name="name">The name of uses-permission.</param>
        /// <param name="toolsNodeValue">The value of tools node.</param>
        public void SetUsesPermission(string name, string toolsNodeValue = null)
        {
            var xpath = $"uses-permission[@android:name='{name}']";
            var element = (XmlElement)manifestElement.SelectSingleNode(xpath, namespaceManager);
            if (element == null)
            {
                element = CreateElement("uses-permission");
                element.SetAttribute("name", AndroidXmlNamespace, name);
                manifestElement.AppendChild(element);
            }

            if (!string.IsNullOrEmpty(toolsNodeValue))
            {
                _ = element.SetAttribute("node", ToolsXmlNamespace, toolsNodeValue);
            }
            else
            {
                element.RemoveAttribute("node", ToolsXmlNamespace);
            }
        }

        /// <summary>
        /// Add or update query package.
        /// </summary>
        /// <param name="name">The name of the package.</param>
        /// <param name="toolsNodeValue">The value of tools node.</param>
        public void SetQueryPackage(string name, string toolsNodeValue = null)
        {
            var queriesElement = GetOrAddQueries();
            string xpath = $"package[@android:name='{name}']";
            var element = (XmlElement)queriesElement.SelectSingleNode(xpath, namespaceManager);
            if (element == null)
            {
                element = CreateElement("package");
                element.SetAttribute("name", AndroidXmlNamespace, name);
                queriesElement.AppendChild(element);
            }

            if (!string.IsNullOrEmpty(toolsNodeValue))
            {
                _ = element.SetAttribute("node", ToolsXmlNamespace, toolsNodeValue);
            }
            else
            {
                element.RemoveAttribute("node", ToolsXmlNamespace);
            }
        }

        /// <summary>
        /// Set application attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The string value of the attribute.</param>
        public void SetApplicationAttribute(string name, string value)
        {
            applicationElement.SetAttribute(name, AndroidXmlNamespace, value);
        }

        private XmlElement GetOrAddQueries()
        {
            var element = (XmlElement)manifestElement.SelectSingleNode("queries");
            if (element == null)
            {
                element = CreateElement("queries");
                manifestElement.AppendChild(element);
            }

            return element;
        }
    }
}
