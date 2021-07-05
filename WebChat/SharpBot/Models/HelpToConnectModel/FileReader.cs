using System.Collections.Generic;
using System.Xml;

namespace WebChat.SharpBot.Models.HelpToConnectModel
{
    public class FileReader
    {
        public static List<string> ReadListFromFiles(string fileName)
        {
            var list = new List<string>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"SharpBot\" + fileName);
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");

                    list.Add(attr.Value);
                }
            }
            return list;
        }
        /// <summary>
        /// Метод на будущее что бы расширять допустим набор приветствий, или слова на которые тригерит приветсвтия
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void WriteOnFile(string fileName, string name, string value)
        {
            XmlDocument xDoc = new XmlDocument();

            xDoc.Load($"{fileName}.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            XmlElement userElem = xDoc.CreateElement(name);
            XmlAttribute nameAttr = xDoc.CreateAttribute("name");
            XmlText nameText = xDoc.CreateTextNode(value);
            userElem.Attributes.Append(nameAttr);
            nameAttr.AppendChild(nameText);
            xRoot.AppendChild(userElem);
            xDoc.Save($"{fileName}.xml");
        }
    }
}
