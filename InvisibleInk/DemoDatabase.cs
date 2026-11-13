using System;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace InvisibleInk
{
    /// <summary>
    /// SQLite database stored in invisibleink.db next to the exe.
    /// If the file doesn't exist, it is created and filled with test data on startup.
    /// Delete the file to reset the demo.
    /// </summary>
    public static class DemoDatabase
    {
        public static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "invisibleink.db");

        public static readonly string ConnectionString = $"Data Source={FilePath}";

        /// <summary>Dates are stored as text in this format, e.g. 13/11/2026.</summary>
        public const string DateFormat = "dd/MM/yyyy";

        /// <summary>SQL expression turning the stored dd/MM/yyyy text into sortable yyyyMMdd.</summary>
        public const string SortableDateSql = "(substr(Date,7,4) || substr(Date,4,2) || substr(Date,1,2))";

        public static string FormatDate(DateTime date) => date.ToString(DateFormat, CultureInfo.InvariantCulture);

        public static string SortableDate(DateTime date) => date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        public static void EnsureCreated()
        {
            if (File.Exists(FilePath))
                return;

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var tx = connection.BeginTransaction();

            Execute(connection, tx, @"
                CREATE TABLE StaffInformation (
                    UserID      INTEGER PRIMARY KEY,
                    Password    TEXT NOT NULL,
                    FullName    TEXT NOT NULL,
                    AccessLevel TEXT NOT NULL
                );
                CREATE TABLE PiercingData (
                    PiercingID   INTEGER PRIMARY KEY AUTOINCREMENT,
                    PiercingType TEXT NOT NULL,
                    Price        REAL NOT NULL
                );
                CREATE TABLE InformationCustomer (
                    CustomerID         INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientName         TEXT NOT NULL,
                    HealthCheckResults TEXT,
                    DOB                TEXT
                );
                CREATE TABLE Bookings (
                    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientName  TEXT NOT NULL,
                    Piercing    TEXT,
                    WhoIsItFor  TEXT,
                    Time        TEXT,
                    Date        TEXT,
                    StaffMember TEXT,
                    NewClient   TEXT,
                    DepositPaid TEXT
                );");

            // Staff. The login screen only asks for the User ID; Password is stored but not checked.
            var staffLogins = new (int Id, string Password, string FullName, string AccessLevel)[]
            {
                (1, "Reception1!",  "Front Desk",    "Receptionist"),
                (2, "Nick2024!",    "Nick",          "Nick"),
                (3, "Oxley2024!",   "Oxley",         "Oxley"),
                (4, "Dave2024!",    "Dave",          "Dave"),
                (5, "Claudia2024!", "Claudia",       "Claudia"),
                (6, "Skelly2024!",  "Skelly",        "Skelly"),
                (7, "Spudgun2024!", "Spudgun",       "Spudgun"),
                (8, "Admin2024!",   "Administrator", "Admin"),
            };
            foreach (var s in staffLogins)
                Execute(connection, tx, "INSERT INTO StaffInformation VALUES (@id, @pw, @name, @level)",
                    ("@id", s.Id), ("@pw", s.Password), ("@name", s.FullName), ("@level", s.AccessLevel));

            var piercings = new (string Type, double Price)[]
            {
                ("Ear Lobe", 20), ("Upper Lobe", 25), ("Helix", 30), ("Forward Helix", 35),
                ("Tragus", 35), ("Anti-Tragus", 35), ("Conch", 35), ("Rook", 40),
                ("Daith", 40), ("Industrial", 45), ("Nostril", 30), ("Septum", 40),
                ("Bridge", 40), ("Eyebrow", 30), ("Labret", 35), ("Medusa", 35),
                ("Monroe", 35), ("Tongue", 40), ("Navel", 40), ("Nipple", 45),
            };
            foreach (var p in piercings)
                Execute(connection, tx, "INSERT INTO PiercingData (PiercingType, Price) VALUES (@t, @p)",
                    ("@t", p.Type), ("@p", p.Price));

            var customers = new (string Name, string Health, string Dob)[]
            {
                ("Jane Smith",    "No allergies, no medical conditions",             "14/03/1998"),
                ("Tom Baker",     "Nickel allergy - titanium jewellery only",        "02/09/2001"),
                ("Priya Patel",   "No issues reported",                              "21/06/1995"),
                ("Sam Jones",     "Type 1 diabetic - extended aftercare discussed",  "30/11/1990"),
                ("Alex Green",    "No issues reported",                              "08/01/2003"),
                ("Chris White",   "Mild eczema - avoid irritated skin areas",        "17/07/1999"),
                ("Olivia Brown",  "No allergies",                                    "25/12/2000"),
                ("Liam Walker",   "Takes blood thinners - GP letter on file",        "11/04/1987"),
                ("Emily Clarke",  "Latex allergy - nitrile gloves only",             "03/10/2002"),
                ("Noah Wright",   "No issues reported",                              "19/02/1996"),
                ("Mia Thompson",  "Prone to keloids - advised against cartilage",    "07/08/1994"),
                ("Jack Robinson", "No allergies",                                    "28/05/2004"),
                ("Ava Hughes",    "Asthma (inhaler with client)",                    "15/09/1997"),
                ("Harry Edwards", "No issues reported",                              "01/03/1992"),
                ("Isla Wood",     "Faints with needles - lying position preferred",  "22/11/2001"),
                ("George Hall",   "No allergies",                                    "09/06/1989"),
                ("Sophie Turner", "Pregnant - piercing postponed until after birth", "13/01/1993"),
                ("Oscar Lewis",   "No issues reported",                              "27/07/2005"),
                ("Grace Martin",  "Penicillin allergy",                              "04/12/1998"),
                ("Leo Harris",    "No allergies",                                    "18/10/2000"),
                ("Ruby Scott",    "Epilepsy - well controlled, no recent seizures",  "06/04/1995"),
                ("Freddie Young", "No issues reported",                              "29/08/2003"),
                ("Lily King",     "Under 18 - parental consent form on file",        "12/02/2008"),
                ("Archie Adams",  "No allergies",                                    "23/03/1991"),
                ("Chloe Baker",   "Sensitive skin - hypoallergenic aftercare",       "16/05/1999"),
            };
            foreach (var c in customers)
                Execute(connection, tx, "INSERT INTO InformationCustomer (ClientName, HealthCheckResults, DOB) VALUES (@n, @h, @d)",
                    ("@n", c.Name), ("@h", c.Health), ("@d", c.Dob));

            SeedBookings(connection, tx, customers, piercings);

            tx.Commit();
        }

        /// <summary>
        /// Bookings from 3 weeks back to 4 weeks ahead of the day the database is created.
        /// Each piercer gets 3-4 bookings on each working day (same days as BookingPortal),
        /// and everyone gets bookings today so "Appointments Today" always shows something.
        /// </summary>
        private static void SeedBookings(SqliteConnection connection, SqliteTransaction tx,
            (string Name, string Health, string Dob)[] customers, (string Type, double Price)[] piercings)
        {
            string[] slots =
            {
                "9:00 AM", "9:20 AM", "9:40 AM", "10:00 AM", "10:20 AM", "10:40 AM",
                "11:00 AM", "11:20 AM", "11:40 AM", "12:00 PM",
                "1:00 PM", "1:20 PM", "1:40 PM", "2:00 PM", "2:20 PM", "2:40 PM",
                "3:00 PM", "3:20 PM", "3:40 PM", "4:00 PM", "4:20 PM", "4:40 PM",
            };

            var staff = new (string Name, DayOfWeek[] Days)[]
            {
                ("Nick",    new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday }),
                ("Oxley",   new[] { DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Sunday }),
                ("Dave",    new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday }),
                ("Claudia", new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday }),
                ("Skelly",  new[] { DayOfWeek.Tuesday, DayOfWeek.Saturday }),
                ("Spudgun", new[] { DayOfWeek.Thursday, DayOfWeek.Friday }),
            };

            using var insert = connection.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText = @"INSERT INTO Bookings (ClientName, Piercing, WhoIsItFor, Time, Date, StaffMember, NewClient, DepositPaid)
                                   VALUES (@c, @p, @w, @t, @d, @s, @n, @dep)";
            foreach (var name in new[] { "@c", "@p", "@w", "@t", "@d", "@s", "@n", "@dep" })
                insert.Parameters.Add(new SqliteParameter(name, ""));

            for (int d = -21; d <= 28; d++)
            {
                DateTime day = DateTime.Today.AddDays(d);
                int dk = d + 100; // keep the arithmetic below non-negative

                for (int s = 0; s < staff.Length; s++)
                {
                    if (d != 0 && Array.IndexOf(staff[s].Days, day.DayOfWeek) < 0)
                        continue;

                    for (int sl = 0; sl < slots.Length; sl++)
                    {
                        // Deterministic spread: roughly 1 slot in 6 is booked.
                        if ((sl * 7 + s * 13 + dk * 11) % 6 != 0)
                            continue;

                        insert.Parameters["@c"].Value = customers[(sl * 5 + s * 7 + dk * 3) % customers.Length].Name;
                        insert.Parameters["@p"].Value = piercings[(sl * 3 + s + dk) % piercings.Length].Type;
                        insert.Parameters["@w"].Value = (sl + s + dk) % 5 == 0 ? "Gift" : "Client";
                        insert.Parameters["@t"].Value = slots[sl];
                        insert.Parameters["@d"].Value = FormatDate(day);
                        insert.Parameters["@s"].Value = staff[s].Name;
                        insert.Parameters["@n"].Value = (sl * 3 + s + dk) % 4 == 0 ? "Yes" : "No";
                        insert.Parameters["@dep"].Value = d < 0 || (sl + s * 2 + dk) % 7 != 0 ? "Yes" : "No";
                        insert.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void Execute(SqliteConnection connection, SqliteTransaction tx, string sql, params (string Name, object Value)[] parameters)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value);
            cmd.ExecuteNonQuery();
        }
    }
}
