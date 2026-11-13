using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvisibleInk
{
    public partial class MainWindow : Window
    {
        public static string mahcine = System.Environment.MachineName;

        private string connectionString = InvisibleInk.DemoDatabase.ConnectionString;
        private UserToSystem currentUser;
        public MainWindow()
        {
            InitializeComponent();
            DemoDatabase.EnsureCreated();
            currentUser = new UserToSystem { UserID = 1 };
        }
        private void GetStaffInfo()
        {
            string enteredUserID = UserIdTextBox.Text;
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM StaffInformation WHERE UserID = @UserID";
                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        // Use SqlParameter to avoid SQL injection
                        command.Parameters.AddWithValue("@UserID", enteredUserID);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Mapping data from the database
                                currentUser.UserID = int.Parse(reader["UserID"].ToString());
                                currentUser.Password = reader["Password"].ToString();
                                currentUser.FullName = reader["FullName"].ToString();
                                currentUser.AccessLevel = reader["AccessLevel"].ToString();

                            }
                            else
                            {
                                MessageBox.Show("User Not Found!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            int enteredUserID;
            if (int.TryParse(UserIdTextBox.Text, out enteredUserID))
            {
                currentUser.UserID = enteredUserID;
                GetStaffInfo();


                if (currentUser.AccessLevel?.Trim() == "Receptionist")
                {
                    NavigateToBookingPortal();
                }

                else if (currentUser.AccessLevel?.Trim() == "Nick")
                {
                    NavigateToNicksPortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Oxley")
                {
                    NavigateToOxleyPortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Dave")
                {
                    NavigateToDavePortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Claudia")
                {
                    NavigateToClaudiaPortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Skelly")
                {
                    NavigateToSkellyPortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Spudgun")
                {
                    NavigateToSpudgunPortal();
                }
                else if (currentUser.AccessLevel?.Trim() == "Admin")
                {
                    NavigateToAdminPortal();
                }
                else
                {
                    MessageBox.Show("Error No User Found");
                }
            }
            else
            {
                MessageBox.Show("Invalid UserID");
            }
        }



        private void NavigateToBookingPortal()
        {
            BookingPortal portal = new BookingPortal(currentUser);
            portal.Show();
            this.Close();

        }

        private void NavigateToNicksPortal()
        {
            Nick nickportal = new Nick(currentUser);
            nickportal.Show();
            this.Close();

        }
        private void NavigateToOxleyPortal()
        {
            Oxley oxleyportal = new Oxley(currentUser);
            oxleyportal.Show();
            this.Close();
        }

        private void NavigateToDavePortal()
        {
            Dave daveport = new Dave(currentUser);
            daveport.Show();
            this.Close();
        }

        private void NavigateToClaudiaPortal()
        { 
            Claudia claudiaport = new Claudia(currentUser);
            claudiaport.Show();
            this.Close();
        }

        private void NavigateToSkellyPortal()
        {
            Skelly skellyport = new Skelly(currentUser);
            skellyport.Show();
            this.Close();
        }
        private void NavigateToSpudgunPortal()
        {
            Spudgun spudgunportal = new Spudgun(currentUser);
            spudgunportal.Show();
            this.Close();
        }
        private void NavigateToAdminPortal()
        {
            AdminPortal adminportal = new AdminPortal(currentUser);
            adminportal.Show();
            this.Close();
        }
        public class UserToSystem
        {
            public int UserID { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string AccessLevel { get; set; }
        }
    }
}



