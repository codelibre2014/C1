using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;

namespace Sistem_Booking_Hotel
{
    class SettingNetwork
    {
        public void EnableTCPIP(string koneksi)
        {
            //string tempCatalog = "master";
            //string temp = @"Data Source=" + dataSource + ";Initial Catalog=" + tempCatalog + ";Integrated Security=True;MultipleActiveResultSets=True";

            SqlConnection sqlconnection = new SqlConnection(koneksi);
            SqlCommand cmd = new SqlCommand("select @@ServerName", sqlconnection);
            sqlconnection.Open();
            string serverName = "";
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    serverName = dr[0].ToString();
            }
            catch
            {
                //MessageBox.Show("Failed to Set SQL Server Properties for remote connections.");
            }

            Server srv = new Server(serverName);
            srv.ConnectionContext.Connect();
            srv.Settings.LoginMode = ServerLoginMode.Mixed;

            ManagedComputer mc = new ManagedComputer();

            try
            {
                Service Mysvc = mc.Services["MSSQL$" + serverName.Split('\\')[1]];

                if (Mysvc.ServiceState == ServiceState.Running)
                {
                    Mysvc.Stop();
                    Mysvc.Alter();

                    while (!(string.Format("{0}", Mysvc.ServiceState) == "Stopped"))
                    {
                        Mysvc.Refresh();
                    }
                }

                ServerProtocol srvprcl = mc.ServerInstances[0].ServerProtocols[2];
                srvprcl.IsEnabled = true;
                srvprcl.Alter();


                Mysvc.Start();
                Mysvc.Alter();

                while (!(string.Format("{0}", Mysvc.ServiceState) == "Running"))
                {
                    Mysvc.Refresh();
                }
            }
            catch
            {
                //MessageBox.Show("TCP/IP connectin could not be enabled.");
            }
        }

        public void EnableFirewallPORT()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "netsh advfirewall firewall add rule name="+ "Open SQL Server Port 1433"+ " dir=in action=allow protocol=TCP localport=1433";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
        }

    }
}
