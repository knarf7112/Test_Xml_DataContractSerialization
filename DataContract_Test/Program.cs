using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//DataContractSerializer
using System.Runtime.Serialization;
namespace DataContract_Test
{
    class Program
    {
        //ref:https://msdn.microsoft.com/zh-tw/library/system.runtime.serialization.extensiondataobject(v=vs.110).aspx
        static void Main(string[] args)
        {
            //目的:將物件序列化成XML標準格式
            try
            {
                //1.寫一個用DataContract序列化的物件(較新的物件:多一個欄位)到檔案
                ExtensionTest.Write_v2("V2.xml");
                //2.覆寫一個用DataContract序列化的物件(較舊的物件)到檔案
                ExtensionTest.Write_v1("V2.xml");
                //3.從檔案讀取並還原回物件
                ExtensionTest.Read_v2("V2.xml");
            }
            catch (System.Runtime.Serialization.SerializationException serEx)
            {
                Console.WriteLine("{0} : {1}", serEx.Message, serEx.StackTrace);
            }
            finally
            {
                Console.ReadKey();
            }


        }
    }
}
