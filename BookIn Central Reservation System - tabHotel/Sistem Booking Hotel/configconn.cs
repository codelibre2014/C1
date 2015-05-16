//using System;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace Sistem_Booking_Hotel
{   
    ///SVN test
    //public class configconn
    //{
    //    public static SqlConnection conn;
    //    public String dataSourceConn;
    //    public int truthVal = 1;
    //    public SqlConnection KoneksiDB()
    //    {
    //        DatabaseConnectionForm dat = new DatabaseConnectionForm();
    //        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Sistem_Booking_Hotel.Properties.Settings.tabHotelConnectionString"].ToString());
    //        try
    //        {
    //            truthVal = 1;
    //            conn.Open();
    //        }
    //        catch (Exception e)
    //        {
    //            truthVal = 0;
    //        }
    //        return conn;
    //    }
    //}

    public class configconn
    {


        public static SqlConnection conn;
        public SqlConnection KoneksiDB()
        {
            conn = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=hoteldb;Integrated Security=True;User ID=NT Service\\MSSQL$SQLEXPRESS;Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;");
            conn.Open();
            return conn;
        }



        //public static SqlConnection conn;
        //public SqlConnection KoneksiDB()
        //{
        //    conn = new SqlConnection("Data Source=tcp:DELL-PC,1433;Initial Catalog=hoteldb;UID=dell;PWD=dell;");
        //    conn.Open();
        //    return conn;
        //}


        //public static SqlConnection conn;
        //public SqlConnection KoneksiDB()
        //{
        //    conn = new SqlConnection("Data Source=DELL-PC;Initial Catalog=hoteldb;Integrated Security=True;User ID=NT Service\\MSSQL$SQLEXPRESS;Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;");
        //    conn.Open();
        //    return conn;
        //}

        //public static SqlConnection conn;
        //public SqlConnection KoneksiDB()
        //{
        //    conn = new SqlConnection("Data Source=tcp:192.168.9.251\\SQLEXPRESS,49172;Initial Catalog=hoteldb;UID=dell;PWD=dell;");
        //    conn.Open();
        //    return conn;
        //}

        //public static SqlConnection conn;
        //public SqlConnection KoneksiDB()
        //{
        //    conn = new SqlConnection("Data Source=PEGASUS;Initial Catalog=hoteldb;Integrated Security=True;User ID=NT Service\\MSSQL$SQLEXPRESS;Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;");
        //    conn.Open();
        //    return conn;
        //}

        //public static SqlConnection conn;
        //public SqlConnection KoneksiDB()
        //{
        //    conn = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=hoteldb;Integrated Security=True;User ID=NT Service\\MSSQL$SQLEXPRESS;Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true;");
        //    conn.Open();
        //    return conn;
        //}

        //public static SqlConnection conn;
        ////string driveLetter = Directory.GetCurrentDirectory();
        //public SqlConnection KoneksiDB()
        //{
        //    /*
        //    string[] line = new string[2];
        //    int ctr = 0;
        //    using (StreamReader reader = new StreamReader(driveLetter + @"\data.blabla"))
        //    {
        //        line[ctr] = reader.ReadLine();
        //        ctr += 1;
        //        line[ctr] = reader.ReadLine();
        //    }*/
        //    XmlDocument XmlDoc = new XmlDocument();
        //    //Loading the Config file
        //    XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        //    foreach (XmlElement xElement in XmlDoc.DocumentElement)
        //    {
        //        if (xElement.Name == "connectionStrings")
        //        {
        //            //setting the coonection string
        //            //xElement.FirstChild.Attributes[1].Value = con;
        //            //Console.WriteLine(xElement.FirstChild.Attributes[1].Value);
        //            conn = new SqlConnection(xElement.FirstChild.Attributes[1].Value);
        //        }
        //    }
        //    //conn = new SqlConnection("Data Source=" + line[0] + ";Initial Catalog=" + line[1] + ";UID=dell;PWD=dell;");
        //    conn.Open();
        //    //Console.WriteLine("Data Source=" + line[0] + ";Initial Catalog=" + line[1] + ";UID=dell;PWD=dell;");
        //    return conn;
        //}

        public void closeConnection()
        {
            conn.Close();
        }

    }

}
