using New_Task_Lab_Wars;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace New_Task_Lab_Wars
{
    internal class ClaDatabase
    {
        //(Connection string should include (Data Source, inital catalog, User UD, Password;)
        //No certificate trusted although may need to test this.
        static readonly string connectionString = InvisibleInk.DemoDatabase.ConnectionString;


        public bool Connection()
        {
            //use class and connection string to create a open connection to the database.
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                //opens the connection for our DB :)
                connection.Open();


                //MessageBox.Show("Works");
                return true;


            }
        }

        public bool AddAccount(ClasLos LoserInfo)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                //opens the connection for our DB :)
                connection.Open();

                //create SQL query#
                string insertUserInfoQuery = "INSERT INTO InformationCustomer (ClientName, HealthCheckResults, DOB) Values (@ClientName, @HealthCheckResults, @DOB)";

                using (SqliteCommand Userdata = new SqliteCommand(insertUserInfoQuery, connection))
                {
                    Userdata.Parameters.AddWithValue("@ClientName", LoserInfo.ClientName);
                    Userdata.Parameters.AddWithValue("@HealthCheckResults", LoserInfo.HealthCheckResults);
                    Userdata.Parameters.AddWithValue("@DOB", LoserInfo.DOB);
                    

                    //run query
                    int rowsAffected = Userdata.ExecuteNonQuery();

                    //why does removing the if and else cause addAccount to get upset
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Customer Information Has been added");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Error Please Check What you have Entered" +
                            "if Issue Persists contact System Admin");
                        return false;
                    }
                }




            }



        }

    }
}