using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace JsonDictionary
{
    class JsonIo
    {
        public static bool SaveJson<T>(T data, string fileName)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(T));
            try
            {
                var fileStream = File.Open(fileName, FileMode.Create);
                jsonSerializer.WriteObject(fileStream, data);
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON parse error: " + ex.Message + Environment.NewLine);
                return false;
            }
            return true;
        }

        public static List<T> LoadJson<T>(string fileName)
        {
            List<T> newValues;
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<T>));
            try
            {
                var fileStream = File.Open(fileName, FileMode.Open);

                newValues = (List<T>)jsonSerializer.ReadObject(fileStream);
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("JSON parse error: " + ex.Message + Environment.NewLine);
                newValues = null;
            }

            return newValues;
        }

        public static void SaveBinary<T>(T tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, tree);
            }
        }

        public static T LoadBinary<T>(string filename)
        {
            T nodeList;
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                var bf = new BinaryFormatter();
                var obj = bf.Deserialize(file);

                nodeList = (T)obj;
            }

            return nodeList;
        }
    }
}
