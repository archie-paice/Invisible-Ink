using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Linq;
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
    /// Interaction logic for Nick.xaml
    /// </summary>
    public partial class Nick : Window
    {
        private UserToSystem currentUser;

        private string connectionString = InvisibleInk.DemoDatabase.ConnectionString;
        public Nick(UserToSystem user)
        {
            InitializeComponent();
            FullNameLabel.Content = $"Hello {user.FullName} Please See Your Schedule";
            currentUser = user;
            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();

            // Replace 'Nick' with the appropriate staff member name
            string staffMemberName = "Nick";

            SqliteCommand cmd = new SqliteCommand($"SELECT * FROM Bookings WHERE StaffMember = '{staffMemberName}'", con);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            DataGrid.ItemsSource = dt.DefaultView;
            lblTotal.Content = $"Total Appointments: {DataGrid.Items.Count-1}";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();

            // Replace 'Nick' with the appropriate staff member name
            string staffMemberName = "Nick";

            // Replace 'fromDate' and 'toDate' with your actual DatePicker values
            DateTime fromDateValue = dtFromDate.SelectedDate ?? DateTime.MinValue;
            DateTime toDateValue = dtToDate.SelectedDate ?? DateTime.MaxValue;

            // Convert date to the string format used in the table (DD/MM/YYYY)
            string fromDateStr = fromDateValue.ToString("dd/MM/yyyy");
            string toDateStr = toDateValue.ToString("dd/MM/yyyy");

            // Use parameters to avoid SQL injection
            string query = $"SELECT * FROM Bookings WHERE StaffMember = @StaffMember AND " + InvisibleInk.DemoDatabase.SortableDateSql + " BETWEEN @FromDate AND @ToDate";

            using (SqliteCommand cmd = new SqliteCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@StaffMember", staffMemberName);
                cmd.Parameters.AddWithValue("@FromDate", InvisibleInk.DemoDatabase.SortableDate(fromDateValue));
                cmd.Parameters.AddWithValue("@ToDate", InvisibleInk.DemoDatabase.SortableDate(toDateValue));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                DataGrid.ItemsSource = dt.DefaultView;

                foreach (var column in DataGrid.Columns)
                {

                    if (column.Header.ToString() == "NewClient" || column.Header.ToString() == "DepositPaid" || column.Header.ToString() == "WhoIsItFor" || column.Header.ToString() == "StaffMember")
                    {
                        column.Visibility = Visibility.Collapsed;

                    }
                }

            }

            lblTotal.Content = $"Total Appointments: {DataGrid.Items.Count-1}";
        }

        private void btnAllAppointments_Click(object sender, RoutedEventArgs e)
        {
            
            
                SqliteConnection con = new SqliteConnection(connectionString);
                con.Open();

                // Replace 'Nick' with the appropriate staff member name
                string staffMemberName = "Nick";

                SqliteCommand cmd = new SqliteCommand($"SELECT * FROM Bookings WHERE StaffMember = '{staffMemberName}'", con);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                DataGrid.ItemsSource = dt.DefaultView;

                lblTotal.Content = $"Total Appointments: {DataGrid.Items.Count-1}";

            

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow exit = new MainWindow();
            exit.Show();
            this.Close();
        }

        private void btnResetGrid_Click(object sender, RoutedEventArgs e)
        { 
            Nick resetgrid = new Nick(currentUser);
            resetgrid.Show();
            this.Close();

        }
        private void btnAllAppointmentsForToday_Click(object sender, RoutedEventArgs e)
        {
            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();

            // Replace 'Nick' with the appropriate staff member name
            string staffMemberName = "Nick";

            // Get today's date
            DateTime today = DateTime.Today;

            // Convert date to the string format used in the table (DD/MM/YYYY)
            string todayStr = today.ToString("dd/MM/yyyy");

            // Use parameters to avoid SQL injection
            string query = $"SELECT * FROM Bookings WHERE StaffMember = @StaffMember AND Date = @Today";

            using (SqliteCommand cmd = new SqliteCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@StaffMember", staffMemberName);
                cmd.Parameters.AddWithValue("@Today", InvisibleInk.DemoDatabase.FormatDate(today));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    // Appointments for today found
                    DataGrid.ItemsSource = dt.DefaultView;
                    lblTotal.Content = $"Total Records: {DataGrid.Items.Count-1}";
                }
                else
                {
                    // No appointments for today
                    MessageBox.Show("Sorry, you have no appointments today Your So Lucky.", "No Appointments", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}
