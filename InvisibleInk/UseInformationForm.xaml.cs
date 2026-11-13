using New_Task_Lab_Wars;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static InvisibleInk.MainWindow;

namespace InvisibleInk
{
    /// <summary>
    /// Interaction logic for UseInformationForm.xaml
    /// </summary>
    public partial class UseInformationForm : Window
    {
        private UserToSystem currentUser;
        private static readonly string ConnectionString = InvisibleInk.DemoDatabase.ConnectionString;

        public UseInformationForm(UserToSystem user)
        {
            InitializeComponent();
            FullNameLabel.Text = $"Hello {user.FullName}";
            currentUser = user;


        }

        private void UpdateUI(UserToSystem currentUser)
        {
            FullNameLabel.Text = currentUser.FullName;

        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ClaDatabase claDatabaseFunctions = new ClaDatabase();

            claDatabaseFunctions.Connection();


            string ClientName = BoxName.Text;
            string HealthCheckResults = BoxDescription.Text;
            string DOB = BoxDOB.Text;



           


            ClasLos loserInfo = new ClasLos();

            loserInfo.ClientName = ClientName;
            loserInfo.HealthCheckResults = HealthCheckResults;
            loserInfo.DOB = DOB;

            claDatabaseFunctions.AddAccount(loserInfo);

            MainWindow home = new MainWindow();
            home.Show();
            this.Close();
        }
    }
}
