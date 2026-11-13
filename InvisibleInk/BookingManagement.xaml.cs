using System;
using System.Collections.Generic;
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
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using System.Net;
using System.Data;

namespace InvisibleInk
{
    /// <summary>
    /// Interaction logic for BookingManagement.xaml
    /// </summary>
    public partial class BookingManagement : Window
    {
        private UserToSystem currentUser;

        private string connectionString = InvisibleInk.DemoDatabase.ConnectionString;

        public BookingManagement(UserToSystem user)
        {
            InitializeComponent();
            FullNameLabel.Content = $"Welcome {user.FullName}";
            currentUser = user;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(BookingRef.Text, out int bookingRef))
            {
                MessageBox.Show("Please enter a valid booking reference number.");
                return;
            }

            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("Update Bookings set ClientName=@ClientName, Piercing=@Piercing, WhoIsItFor=@WhoIsItFor, Time=@Time, Date=@Date, StaffMember=@StaffMember, NewClient=@NewClient, DepositPaid=@DepositPaid WHERE ID = @BookingReference", con);
            cmd.Parameters.AddWithValue("@BookingReference", bookingRef);
            cmd.Parameters.AddWithValue("@ClientName",ClientName.Text);
            cmd.Parameters.AddWithValue("@Piercing", PiercingName.Text);
            cmd.Parameters.AddWithValue("@WhoIsItFor", whofor.Text);
            cmd.Parameters.AddWithValue("@Time", timeday.Text);
            cmd.Parameters.AddWithValue("@Date", date.Text);
            cmd.Parameters.AddWithValue("@StaffMember", AssighnedStaffMember.Text);
            cmd.Parameters.AddWithValue("@NewClient", NewClient.Text);
            cmd.Parameters.AddWithValue("@DepositPaid", DepositPaid.Text);

            int rowsUpdated = cmd.ExecuteNonQuery();
            con.Close();

            if (rowsUpdated == 0)
            {
                MessageBox.Show($"No booking found with reference {bookingRef}.");
                return;
            }
            MessageBox.Show("Updated Booking Complete!");
            BookingRef.Text = String.Empty;
            ClientName.Text = String.Empty;
            PiercingName.Text = String.Empty;
            whofor.Text = String.Empty;
            timeday.Text = String.Empty;
            date.Text = String.Empty;
            AssighnedStaffMember.Text = String.Empty;
            NewClient.Text = String.Empty;
            DepositPaid.Text = String.Empty;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(BookingRef.Text, out int bookingRef))
            {
                MessageBox.Show("Please enter a valid booking reference number.");
                return;
            }

            if (MessageBox.Show("Are you want to Delete/Cancel Booking?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Error) != MessageBoxResult.Yes)
            {
                return;
            }

            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("DELETE FROM Bookings WHERE ID = @BookingReference", con);
            cmd.Parameters.AddWithValue("@BookingReference", bookingRef);
            int rowsDeleted = cmd.ExecuteNonQuery();
            con.Close();

            if (rowsDeleted == 0)
            {
                MessageBox.Show($"No booking found with reference {bookingRef}.");
                return;
            }
            BookingRef.Text = String.Empty;
            ClientName.Text = String.Empty;
            PiercingName.Text = String.Empty;
            whofor.Text = String.Empty;
            timeday.Text = String.Empty;
            date.Text = String.Empty;
            AssighnedStaffMember.Text = String.Empty;
            NewClient.Text = String.Empty;
            DepositPaid.Text = String.Empty;
            MessageBox.Show("Booking Has Been deleted");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SqliteConnection con = new SqliteConnection(connectionString);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("Select * from Bookings",con);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            DataGrid.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
