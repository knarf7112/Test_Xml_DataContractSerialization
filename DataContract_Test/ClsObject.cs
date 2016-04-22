using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Runtime.Serialization;

namespace DataContract_Test
{
    // Implement the IExtensibleDataObject interface 
    // to store the extra data for future versions. 使用DataContract屬性來宣告此物件,Name和命名空間要自訂,這會在DataContractSerializer時使用
    [DataContract(Name="Person", Namespace="Test.MyDefine")]
    public class Person : IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataObject_value;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataObject_value;
            }
            set
            {
                this.extensionDataObject_value = value;
            }
        }
        [DataMember]
        public string Name;
    }
    //物件的第二代之類的  多了一個欄位成員
    [DataContract(Name = "Person", Namespace = "Test.MyDefine")]
    public class Person_v2 : IExtensibleDataObject
    {
        //表示序列化時的順序
        [DataMember(Order=2)]
        public int ID;
        private ExtensionDataObject extensiondataObject_value;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensiondataObject_value;
            }
            set
            {
                this.extensiondataObject_value = value;
            }
        }
        //表示為序列化的成員
        [DataMember]
        public string Name;
    }


    public static class ExtensionTest
    {
        public static void Write_v1(string path)
        {
            //使用較舊的物件來讀取Xml檔內的數據(較新的物件:多一個屬性)
            DataContractSerializer serializer = new DataContractSerializer(typeof(Person));
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);

            System.Xml.XmlDictionaryReader xmlrd = System.Xml.XmlDictionaryReader.CreateTextReader(fs, new System.Xml.XmlDictionaryReaderQuotas());
            Console.WriteLine("Deserializing v2 data to v1 object. \n\n");
            Person p1 = serializer.ReadObject(xmlrd, false) as Person;//從XmlDictionaryReader讀取資料流
            //Person_v2 p2 = serializer.ReadObject(xmlrd, false) as Person_v2;//轉不回來較新的物件=>null
            Console.WriteLine("V1 data has only a Name field.");
            Console.WriteLine("But the v2 data is stored in the ");
            Console.WriteLine("ExtensionData property. \n\n");
            Console.WriteLine("\t Name: {0} \n", p1.Name);

            fs.Close();
            // 改變欄位值並覆寫回物件
            // Change data in the object.
            p1.Name = "John";
            Console.WriteLine("Changed the Name value to 'John' ");
            Console.Write("and reserializing the object to version 2 \n\n");
            // Reserialize the object.
            fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
            serializer.WriteObject(fs, p1);
            fs.Close();

        }

        public static void Write_v2(string path)
        {
            Console.WriteLine("Creating a version 2 object");

            Person_v2 p2 = new Person_v2();
            p2.Name = "Elizabeth";
            p2.ID = 2006;

            Console.WriteLine("Object data includes an ID");
            Console.WriteLine("\t Name: {0}", p2.Name);
            Console.WriteLine("\t ID: {0} \n", p2.ID);
            // Create an instance of the DataContractSerializer.
            //依靠DataContractSerializer來做序列化寫入(先告訴此物件要序列化的物件類別)
            DataContractSerializer serializer = new DataContractSerializer(typeof(Person_v2));

            Console.WriteLine("Serializing the v2 object to a file. \n\n");
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
            serializer.WriteObject(fs, p2);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();//用MemoryStream來看看真實數據
            serializer.WriteObject(ms, p2);
            byte[] data = ms.ToArray();
            Console.WriteLine("物件轉成XML數據的內容:\n{0}", Encoding.ASCII.GetString(data));
            ms.Close();
            fs.Close();

        }

        public static void Read_v2(string path)
        {
            //從資料流取得檔案
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            //並使用DataContractSerializer還原回物件
            DataContractSerializer serializer = new DataContractSerializer(typeof(Person_v2));
            Console.WriteLine("Deserializing new data to version 2 \n\n");

            Person_v2 p2 = serializer.ReadObject(fs) as Person_v2;

            fs.Close();

            Console.WriteLine("The data includes the old ID field value. \n");
            Console.WriteLine("\t (New) Name: {0}", p2.Name);
            Console.WriteLine("\t ID: {0} \n", p2.ID);
        }
    }
}
