using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
using static InvisibleInk.BookingPortal;
using static InvisibleInk.MainWindow;

namespace InvisibleInk
{
    /// <summary>
    /// Interaction logic for BookingPortal.xaml
    /// </summary>
    public partial class BookingPortal : Window
    {

        private UserToSystem currentUser;

        private string connectionString = InvisibleInk.DemoDatabase.ConnectionString;

        public class Employee
        {
            public string Name { get; set; }
            public List<DayOfWeek> WorkingDays { get; set; }
            public List<TimeSpan> Appointments { get; set; }

            public List<DayOfWeek> AvailableDays { get; set; }

            public override string ToString()
            {
                return Name; // Return the employee's name as the string representation
            }
        }

        private List<Employee> employees;

        private void ClientSearchListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changed event here if needed
        }

        public BookingPortal(UserToSystem user)
        {
            InitializeComponent();
            FullNameLabel.Text = $"Welcome {user.FullName}";
            currentUser = user;

            employees = new List<Employee>
            {
                 new Employee { Name = "Nick", WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday } },
                 new Employee { Name = "Oxley", WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Sunday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Sunday } },
                 new Employee { Name = "Dave", WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday } },
                 new Employee { Name = "Claudia", WorkingDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday } },
                 new Employee { Name = "Skelly", WorkingDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Saturday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Saturday } },
                 new Employee { Name = "Spudgun", WorkingDays = new List<DayOfWeek> { DayOfWeek.Thursday, DayOfWeek.Friday }, Appointments = new List<TimeSpan> { TimeSpan.Zero, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(40) }, AvailableDays = new List<DayOfWeek> { DayOfWeek.Thursday, DayOfWeek.Friday } },
                // Add more employees if more staff is needed
            };


            PopulatePiercingChoices();

        }


        


        private void ClientName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Perform client search based on the entered text
            string searchText = ClientName.Text;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                SearchClients(searchText);
            }
            else
            {
                // Clear the list when no search text is entered
                clientSearchListBox.Items.Clear();
            }
        }

        private void SearchClients(string searchText)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to search for clients with the specified name
                    string query = $"SELECT DISTINCT ClientName FROM InformationCustomer WHERE ClientName LIKE '%{searchText}%'";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Clear existing items in the ListBox
                            clientSearchListBox.Items.Clear();

                            // Add each matching client name to the ListBox
                            while (reader.Read())
                            {
                                string clientName = reader["ClientName"].ToString();
                                clientSearchListBox.Items.Add(clientName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulatePiercingChoices()
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to retrieve PiercingType from PiercingData table
                    string query = "SELECT PiercingType FROM PiercingData";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Clear existing items in the ListBox
                            PiercingChoice.Items.Clear();

                            // Add each PiercingType to the ListBox
                            while (reader.Read())
                            {
                                string piercingType = reader["PiercingType"].ToString();
                                PiercingChoice.Items.Add(piercingType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            // Filter employees based on the selected date and time
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue;
            DayOfWeek selectedDayOfWeek = selectedDate.DayOfWeek;

            string selectedTimeString = ((ComboBoxItem)timeComboBox.SelectedItem)?.Content.ToString();
            try
            {
                DateTime selectedTime = DateTime.ParseExact(selectedTimeString, "h:mm tt", null);
                List<Employee> filteredEmployees = new List<Employee>();
                foreach (var employee in employees)
                {
                    if (employee.WorkingDays.Contains(selectedDayOfWeek) &&
                        employee.AvailableDays.Contains(selectedDayOfWeek) && // Check staff availability
                        IsWorkingHours(selectedTime) &&
                        IsAvailableTimeSlot(employee, selectedTime))
                    {
                        filteredEmployees.Add(employee);
                    }
                }

                // Update the ComboBox with filtered employees
                employeeCombobox.ItemsSource = filteredEmployees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Please Select Time Of Appointment Before Date", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private bool IsWorkingHours(DateTime selectedTime)
        {
            // Check if the selected time is within working hours (9:00 AM to 5:00 PM)
            return selectedTime.TimeOfDay >= TimeSpan.FromHours(9) && selectedTime.TimeOfDay < TimeSpan.FromHours(17);
        }

        private bool IsAvailableTimeSlot(Employee employee, DateTime selectedTime)
        {
            // Check if the employee has fewer than three appointments at the selected time
            int appointmentsAtSelectedTime = GetAppointmentsAtTime(employee, selectedTime);
            return appointmentsAtSelectedTime < 3;
        }

        private int GetAppointmentsAtTime(Employee employee, DateTime selectedTime)
        {
            // For simplicity, assuming appointments start at 00, 20, and 40 minutes past the hour
            return employee.Appointments.Count(appointment =>
                selectedTime.TimeOfDay == appointment ||
                selectedTime.TimeOfDay == appointment.Add(TimeSpan.FromMinutes(20)) ||
                selectedTime.TimeOfDay == appointment.Add(TimeSpan.FromMinutes(40)));
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            // Check if an existing client is selected from the search results
            if (clientSearchListBox.SelectedItem != null)
            {
                // Existing client selected from the search results
                string selectedClient = clientSearchListBox.SelectedItem.ToString();
                HandleBooking(selectedClient);
            }
            else
            {
                // New client entered in the TextBox
                string newClient = ClientName.Text;
                HandleBooking(newClient);
            }
        }

        private void HandleBooking(string clientName)
        {
            // Determine whether the customer is new or existing
            bool isNewCustomer = clientSearchListBox.SelectedItem == null;

            // Get the selected piercing choice from the PiercingChoice ListBox
            string selectedPiercingChoice = PiercingChoice.SelectedItem?.ToString() ?? "N/A";

            // Retrieve the price information based on the selected piercing choice
            string totalPrice = GetTotalPrice(selectedPiercingChoice);

            // Puts staff name in a variable
            string selectedStaff = employeeCombobox.SelectedValue?.ToString();

            string selectedDate = (datePicker.SelectedDate.HasValue ? InvisibleInk.DemoDatabase.FormatDate(datePicker.SelectedDate.Value) : null) ?? "N/A";
            string selectedTime = ((ComboBoxItem)timeComboBox.SelectedItem)?.Content?.ToString() ?? "N/A";

            if (isNewCustomer)
            {
                // Logic for new customer
                // ...

                // Display the selected appointment information, client name, piercing choice, and total price in the ListBox
                string customerInfo = $"Client: {clientName}, Date: {selectedDate}, Time: {selectedTime}, Piercing Choice: {selectedPiercingChoice} Staff Member {selectedStaff} Price: {totalPrice}";
                outputListBox.Items.Add(customerInfo);
                // Update the label with booking details and total price
                lblBookingDetails.Content = $"Total Price For Piercing: {totalPrice:C}";
            }
            else
            {
                // Logic for existing customer
                // ...

                // Display the selected appointment information, client name, piercing choice, and total price in the ListBox
                string customerInfo = $"Client: {clientName}, Date: {selectedDate}, Time: {selectedTime}, Piercing Choice: {selectedPiercingChoice} Staff Member {selectedStaff} Price: {totalPrice}";
                outputListBox.Items.Add(customerInfo);
                // Update the label with booking details and total price
                lblBookingDetails.Content = $"Total Price For Piercing: {totalPrice:C}";
            }
        }


        private void ClearBooking_Click(object sender, RoutedEventArgs e)
        {
            // Clear the items in the outputListBox
            outputListBox.Items.Clear();
            lblBookingDetails.Content = "";
        }

        private string GetTotalPrice(string piercingChoice)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to retrieve Price from PiercingData table based on the selected piercing choice
                    string query = $"SELECT Price FROM PiercingData WHERE PiercingType = '{piercingChoice}'";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToDecimal(result).ToString("0.00");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving price: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Return 0 if price retrieval fails
            return null;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindowq = new MainWindow();
            mainWindowq.Show();
            this.Close();
        }

        private void btnAddNewBooking_Click(object sender, RoutedEventArgs e)
        {
            // Puts staff name in a variable
            string SelectedStaff = employeeCombobox.SelectedValue?.ToString();


            // Get the necessary data from the UI elements
            string clientName = ClientName.Text;
            string selectedPiercingChoice = PiercingChoice.SelectedItem?.ToString() ?? "N/A";
            string selectedDate = (datePicker.SelectedDate.HasValue ? InvisibleInk.DemoDatabase.FormatDate(datePicker.SelectedDate.Value) : null) ?? "N/A";
            string selectedTime = ((ComboBoxItem)timeComboBox.SelectedItem)?.Content?.ToString() ?? "N/A";
            string staffMember = SelectedStaff;
            string whoIsItFor = BoxWhoFor.Text;
            string newClient = NewClient.IsChecked == true ? "Yes" : "No";
            string depositPaid = "Yes"; // Assuming the deposit is paid

            // Calculate total price based on the selected piercing choice
            string totalPrice = GetTotalPrice(selectedPiercingChoice);

            // Generate a unique booking reference (you may want to implement this differently)
            string bookingReference = Guid.NewGuid().ToString();

            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // Check if there is already an appointment for the selected StaffMember at the specified Date and Time
                    string checkExistingAppointmentQuery = @"SELECT COUNT(*) 
                                                 FROM Bookings 
                                                 WHERE StaffMember = @StaffMember 
                                                   AND Date = @Date 
                                                   AND Time = @Time";

                    using (SqliteCommand checkCommand = new SqliteCommand(checkExistingAppointmentQuery, connection))
                    {
                        // Add parameters to the query
                        checkCommand.Parameters.AddWithValue("@StaffMember", staffMember);
                        checkCommand.Parameters.AddWithValue("@Date", selectedDate);
                        checkCommand.Parameters.AddWithValue("@Time", selectedTime);

                        // Execute the query to check for existing appointments
                        int existingAppointmentsCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingAppointmentsCount == 0)
                        {
                            // No existing appointment, proceed with insertion

                            // SQL query to insert data into the Bookings table
                            string insertQuery = @"INSERT INTO Bookings 
                                       (ClientName, Piercing, WhoIsItFor, Time, Date, StaffMember, NewClient, DepositPaid) 
                                       VALUES 
                                       (@ClientName, @Piercing, @WhoIsItFor, @Time, @Date, @StaffMember, @NewClient, @DepositPaid)";

                            using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                            {
                                // Add parameters to the query
                                command.Parameters.AddWithValue("@ClientName", clientName);
                                command.Parameters.AddWithValue("@Piercing", selectedPiercingChoice);
                                command.Parameters.AddWithValue("@WhoIsItFor", whoIsItFor);
                                command.Parameters.AddWithValue("@Time", selectedTime);
                                command.Parameters.AddWithValue("@Date", selectedDate);
                                command.Parameters.AddWithValue("@StaffMember", staffMember);
                                command.Parameters.AddWithValue("@NewClient", newClient);
                                command.Parameters.AddWithValue("@DepositPaid", depositPaid);

                                // Execute the query
                                command.ExecuteNonQuery();

                                // Display success message
                                MessageBox.Show("Booking confirmed and added to the system. Please give the customer their appointment card.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            // Existing appointment found, show a message to the user
                            MessageBox.Show("Appointment already exists for the selected StaffMember at the specified Date and Time. Please choose a different time or staff member.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDisplayBooking_Click(object sender, RoutedEventArgs e)
        {


            ///Display This Latter
        }

        private void btndeletecurrent_Click(object sender, RoutedEventArgs e)
        {


        }

        private void bookingchange_Click(object sender, RoutedEventArgs e)
        {
            BookingManagement changebooking = new BookingManagement(currentUser);
            changebooking.Show();

        }

        private void NewClient_Checked(object sender, RoutedEventArgs e)
        {
            UseInformationForm useInformationForm = new UseInformationForm(currentUser);
            useInformationForm.Show();
            this.Close();
        }
    }
}