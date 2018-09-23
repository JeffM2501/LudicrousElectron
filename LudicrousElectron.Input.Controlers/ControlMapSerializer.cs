using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace LudicrousElectron.Input.Controlers
{
    public static class ControlMapSerializer
    {
        public static string LastError = string.Empty;

        public static ControlMapping Load(string fileName)
        {
            try
            {
                XmlSerializer XML = new XmlSerializer(typeof(ControlMapping));

                FileStream fs = File.OpenRead(fileName);
                ControlMapping ctl = XML.Deserialize(fs) as ControlMapping;
                fs.Close();

                if (ctl == null)
                    LastError = "Read Failed";

                return ctl;

            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
               
            }

            return null;
        }

        public static bool Save(string fileName, ControlMapping mapping)
        {
            try
            {
                XmlSerializer XML = new XmlSerializer(typeof(ControlMapping));

                if (File.Exists(fileName))
                    File.Delete(fileName);

                FileStream fs = File.OpenWrite(fileName);
                XML.Serialize(fs, mapping);
                fs.Close();

                return true;

            }
            catch (Exception ex)
            {
                LastError = ex.ToString();

            }

            return false;
        }
    }
}
