
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Sistem_Booking_Hotel
{
    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {
        private string logFilePath = "C:\\SetupLog.txt";
        public CustomInstaller()
        {
            //This call is required by the Component Designer.
            //Add initialization code after the call to InitializeComponent
            InitializeComponent();
        }

        private string GetSql(string Name)
        {

            try
            {
                // Gets the current assembly.
                Assembly Asm = Assembly.GetExecutingAssembly();
                //Log(Name);
                //Log((Asm.GetName().Name + "." + Name));
                // Resources are named using a fully qualified name.
                Stream strm = Asm.GetManifestResourceStream("Sistem_Booking_Hotel" + "." + Name);

                // Reads the contents of the embedded file.
                StreamReader reader = new StreamReader(strm);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                throw ex;
            }
        }

        private void ExecuteSql(string serverName, string dbName, string Sql)
        {
            string connStr = "Data Source=" + serverName + ";Initial Catalog=" + dbName + ";uid=NT SERVICE\\MSSQLSERVER;Integrated Security=True;";
            Log("Conn Str : " + connStr);

            //using (SqlConnection conn = new SqlConnection(connStr))
            //{
            try
            {
                SqlConnection conn1 = new SqlConnection(connStr);

                //Server server = new Server(new ServerConnection(conn1));

                //server.ConnectionContext.ExecuteNonQuery(Sql);

                IEnumerable<string> commandStrings = Regex.Split(Sql, @"^\s*GO\s*$",
                       RegexOptions.Multiline | RegexOptions.IgnoreCase);

                conn1.Open();
                Log("Connection Success!");
                foreach (string commandString in commandStrings)
                {
                    Log(commandString);
                    if (commandString.Trim() != "")
                    {
                        new SqlCommand(commandString, conn1).ExecuteNonQuery();
                    }
                }
                conn1.Close();

                //Server server = new Server(new ServerConnection(conn));
                //server.ConnectionContext.ExecuteNonQuery(Sql);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            //}
        }
        protected void AddDBTable(string serverName)
        {
            try
            {
                // Creates the database and installs the tables.          
                string strScript = GetSql("sql.txt");
                //Log("SQL : "+strScript);
                ExecuteSql(serverName, "master", strScript);
                Log("Execute SQL Done !! ");
            }
            catch (Exception ex)
            {
                //Reports any errors and abort.
                Log(ex.ToString());
                throw ex;
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            Log("Setup started");
            //Log("Servername : " + this.Context.Parameters["servername"]);
            //AddDBTable(this.Context.Parameters["servername"]);
            AddDBTable(".\\SQLEXPRESS");

        }
        public void Log(string str)
        {
            StreamWriter Tex;
            try
            {
                Tex = File.AppendText(this.logFilePath);
                Tex.WriteLine(DateTime.Now.ToString() + " " + str);
                Tex.Close();
            }
            catch
            { }
        }
    }
}
