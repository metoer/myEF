using DataBase.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Configure_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenMachineConfiguration();//.OpenExeConfiguration(ConfigurationUserLevel.None);
            //string value =config.AppSettings.Settings["DBType"].Value;
            //string test =config.ConnectionStrings.ConnectionStrings["PostgreSQL-1"].ToString();
            //config.ConnectionStrings.ConnectionStrings["PostgreSQL-1"] = "xcvsdf";
            //config.AppSettings.Settings["DBType"].Value = "aa";
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
          //  bool result = Common.CheckDataBaseExist("192.168.124.57", "5432", "postgres", "123456", "postgres");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Loading loading = new Loading();
            loading.Owner = this;
            loading.ShowDialog();
        }
    }
}
