using System;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Data.Sql;

namespace Sistem_Booking_Hotel
{
    public partial class DatabaseConnectionForm : Form
    {
        public String strCon; //= "Data Source=LYCURGUS\\DEMO3;Initial Catalog=tabHotels;Integrated Security=True";

        public DatabaseConnectionForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            string driveLetter = Directory.GetCurrentDirectory();
            string[] lines = new string[2];
            lines[0] = textBox1.Text;
            lines[1] = textBox2.Text;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(driveLetter + @"\data.blabla"))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line);
                }
            }
            this.Close();
            */
            try
            {
                //Constructing connection string from the inputs
                StringBuilder Con = new StringBuilder("Data Source=");
                Con.Append(serverName.Text);
                Con.Append(";Initial Catalog=");
                Con.Append(databaseNamee.Text);
                if (WindowsAuthen.Checked)
                {
                    Con.Append(";Integrated Security=True;");
                }
                else
                {
                    Con.Append(";UID="+UID.Text+";PWD="+PWD.Text+";");
                }
                string strCon = Con.ToString();
                //Console.WriteLine(strCon);
                updateConfigFile(strCon);
                this.Close();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

        }

        public void updateConfigFile(string con)
        {
            //updating config file
            XmlDocument XmlDoc = new XmlDocument();
            //Loading the Config file
            XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            foreach (XmlElement xElement in XmlDoc.DocumentElement)
            {
                if (xElement.Name == "connectionStrings")
                {
                    //setting the coonection string
                    //xElement.FirstChild.Attributes[1].Value = con;
                    foreach (XmlNode childNode in xElement.ChildNodes)
                    {
                        childNode.Attributes[1].Value = con;
                    }
                    //Console.WriteLine(xElement.FirstChild.Attributes[1].Value);
                }

            }
            //writing the connection string in config file
            XmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private void DatabaseConnectionForm_Load(object sender, EventArgs e)
        {
            WindowsAuthen.Checked = true;
            serverName.Items.Clear();

            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            System.Data.DataTable table = instance.GetDataSources();
            foreach (System.Data.DataRow row in table.Rows)
            {
                if (row["ServerName"] != DBNull.Value && Environment.MachineName.Equals(row["ServerName"].ToString()))
                {
                    string item = string.Empty;
                    item = row["ServerName"].ToString();
                    if (row["InstanceName"] != DBNull.Value || !string.IsNullOrEmpty(Convert.ToString(row["InstanceName"]).Trim()))
                    {
                        item += @"\" + Convert.ToString(row["InstanceName"]).Trim();
                    }
                    serverName.Items.Add(item);
                    //Console.WriteLine(item);
                    /*using (var con = new SqlConnection("Data Source=" + item + "; Integrated Security=True;"))
                    {
                        try
                        {
                            con.Open();
                            DataTable databases = con.GetSchema("Databases");
                            foreach (DataRow database in databases.Rows)
                            {
                                String databaseName = database.Field<String>("database_name");
                                short dbID = database.Field<short>("dbid");
                                DateTime creationDate = database.Field<DateTime>("create_date");
                                Console.WriteLine(item +";"+ databaseName);
                            }
                        }
                        catch { }
                    }*/ 
                }
                //Console.WriteLine(row["ServerName"]);
            }
        }

        private void WindowsAuthen_CheckedChanged(object sender, EventArgs e)
        {
            if (WindowsAuthen.Checked)
            {
                groupBoxUser.Enabled = false;
            }
            else
            {
                groupBoxUser.Enabled = true;
            }
        }

        private void databaseNamee_DropDown(object sender, EventArgs e)
        {
            databaseNamee.Items.Clear();
            StringBuilder Con = new StringBuilder("Data Source=");
            Con.Append(serverName.Text);
            if (WindowsAuthen.Checked)
            {
                Con.Append(";Integrated Security=True;");
            }
            else
            {
                Con.Append(";UID=" + UID.Text + ";PWD=" + PWD.Text + ";");
            }
            SqlConnection conn = new SqlConnection(Con.ToString());
            try
            {
                conn.Open();
                DataTable databases = conn.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    //short dbID = database.Field<short>("dbid");
                    //DateTime creationDate = database.Field<DateTime>("create_date");
                    //Console.WriteLine(serverName.Text + ";" + databaseName);
                    databaseNamee.Items.Add(databaseName);
                }
            }
            catch { }
        }

    }
}
