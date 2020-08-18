using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace bingoLibrary
{
    public class bingoLib
    {
        // Used to access database values for a particular object of the class
        string fName, lName, username, password, cardInfo, cvv, balance;
        int id;
        SqlDataAdapter sda;
        SqlCommandBuilder scb;
        DataTable dt;

        public int Id
        {
            get { return id; }
            set { id = value; }

        }
        public string Fname
        {
            get { return fName; }
            set { fName = value; }
        }

        public string Lname
        {
            get { return lName; }
            set { lName = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string CardInfo
        {
            get { return cardInfo; }
            set { cardInfo = value; }
        }

        public string Cvv
        {
            get { return cvv; }
            set { cvv = value; }
        }

        public string Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        //Received the credentials the user inputed in the login window
        public bingoLib GetAccount(string user, string pass)
        {
            bingoLib account = new bingoLib(); // Object that will be returned with all the account information
            //Connection to the database. Edit it with the bingoAccounts.mdf location on the Solution Explorer
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\caraz\source\repos\bingoLibrary\bingoLibrary\bingoAccounts.mdf;Integrated Security=True");
            // SELECT query
            SqlCommand cmd = new SqlCommand("SELECT * from Accounts WHERE username='" + user+"' AND password='"+pass+"'", con);

            using (con) // Open the connection
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) // Read all the information the database returned
                {
                    account.Id = (int)reader["Id"];
                    account.Username = (string)reader["username"];
                    account.Password = (string)reader["password"];
                    account.Fname = (string)reader["fname"];
                    account.Lname = (string)reader["lname"];
                    account.CardInfo = (string)reader["cardInfo"];
                    account.Cvv = (string)reader["cvv"];
                    account.Balance = (string)reader["balance"];
                }
            }
            con.Close();

            return account; // Return the object with all the information
        }

        // Used when the user sign's up for an account or edits their username
        public int CheckUsername(bingoLib acc, int op)
        {
            SqlCommand cmd;
            string validate = ""; // Value of the returned database query gets stored here
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\caraz\source\repos\bingoLibrary\bingoLibrary\bingoAccounts.mdf;Integrated Security=True");

            //For user sign up
            if (op == 0)
            {
                //Check all the accounts in the database
                cmd = new SqlCommand("SELECT username from Accounts WHERE username='" + acc.Username + "'", con);
                using (con)
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Get the username from the database
                    while (reader.Read())
                    {
                        validate = (string)reader["username"];
                    }
                }
                con.Close();
            }

            //For user editted username
            else if (op == 1)
            {
                //Check through the accounts that do not have this ID
                cmd = new SqlCommand("SELECT username from Accounts WHERE username='" + acc.Username + "' AND Id != "+acc.Id, con);
                using (con)
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Get the username from the database
                    while (reader.Read())
                    {
                        validate = (string)reader["username"];
                    }
                }
                con.Close();
            }

            // If no duplicate username, then the username is valid
            if (string.IsNullOrEmpty(validate))
                return 0;

            // If matching username, invalid
            else
                return 1;
        }

        //When the user decides to sign up for an account
        public int AddAccount(bingoLib newAcc)
        {
            // Only letters followed by ONE hyphen/space and ending in a letter
            var regexItem = new Regex(@"^([A-Z][a-z]?[A-Z]?[a-z]+)((-|\s)[A-Z][a-z][A-Z]?[a-z]+)?$");

            // Can not contain spaces
            var upregexItem = new Regex(@"\s");

            // Card number must be 16-digits long
            var cardregex = new Regex(@"^(\d{16})$");

            // CVV must be 3-digits long
            var cvvregex = new Regex(@"^(\d{3})$");

            // If there are not only letters or a hyphen, then input is invalid for First/Last Name
            if (!regexItem.IsMatch(newAcc.Fname) || !regexItem.IsMatch(newAcc.Lname) || newAcc.Fname[newAcc.Fname.Length - 1] == ' ' || newAcc.Lname[newAcc.Lname.Length - 1] == ' ')
                return 0; // Specific error message number

            // If there are spaces then the Username or Password is invalid
            else if (upregexItem.IsMatch(newAcc.Username) || upregexItem.IsMatch(newAcc.Password))
                return 1; // Specific error message number

            // If the card information is not 16-digits long
            else if (!cardregex.IsMatch(newAcc.CardInfo))
                return 2;

            // If the CVV is not 3-digits long
            else if (!cvvregex.IsMatch(newAcc.Cvv))
                return 3;

            // If the username is not unique
            else if (newAcc.CheckUsername(newAcc,0) == 1)
                return 4;

            // If the starting balance is higher than $1200
            else if (Convert.ToInt32(newAcc.Balance) > 1200)
                return 5;

            // No problems found, add this new record to the database
            else
            {
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\caraz\source\repos\bingoLibrary\bingoLibrary\bingoAccounts.mdf;Integrated Security=True");
                string value = "('"+newAcc.Username+"', '"+newAcc.Password+"', '"+newAcc.Fname+"', '"+newAcc.Lname+"', '"+newAcc.CardInfo+"', '"+newAcc.Cvv+"', '"+newAcc.Balance+"');";
                sda = new SqlDataAdapter("INSERT INTO Accounts (username, password, fname, lname, cardInfo, cvv, balance) VALUES " + value, con);
                dt = new DataTable();
                sda.Fill(dt);

                scb = new SqlCommandBuilder(sda);
                sda.Update(dt);
                return 6; // Success number
            }
        }

        // Tracks the counters in Form2 to check if there is a column or row equal to 5
        public int CheckBingo(int one, int two, int three, int four, int five)
        {
            // If the coluumn/row value is equal to 5, bingo is achieved
            if (one == 5 || two == 5 || three == 5 || four == 5 || five == 5)
                return 1;

            else
                return 0;
        }

        // Used only to update the account's balance. All the checking gets done in the form
        public void UpdateBalance(bingoLib acc)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\caraz\source\repos\bingoLibrary\bingoLibrary\bingoAccounts.mdf;Integrated Security=True");
            sda = new SqlDataAdapter("UPDATE Accounts SET balance = '"+acc.Balance+"' WHERE Id = " + acc.Id.ToString(), con);
            dt = new DataTable();
            sda.Fill(dt);

            scb = new SqlCommandBuilder(sda);
            sda.Update(dt);
        }

        // Similar to AddAccount, uses the same regular expressions
        public int UpdateAccountInfo(bingoLib acc)
        {
            // Only letters followed by ONE hyphen/space and ending in a letter
            var regexItem = new Regex(@"^([A-Z][a-z][A-Z]?[a-z]+)((-|\s)[A-Z][a-z][A-Z]?[a-z]+)?$");

            // Can not contain spaces
            var upregexItem = new Regex(@"\s");

            // Card information must be 16-digits long
            var cardregex = new Regex(@"^(\d{16})$");

            // CVV number should only be 3-digits long
            var cvvregex = new Regex(@"^(\d{3})$");

            // If there are not only letters or a hyphen, then input is invalid for First/Last Name
            if (!regexItem.IsMatch(acc.Fname) || !regexItem.IsMatch(acc.Lname) || acc.Fname[acc.Fname.Length - 1] == ' ' || acc.Lname[acc.Lname.Length - 1] == ' ')
                return 0; // Specific error message number

            // If there are spaces then the Username or Password is invalid
            else if (upregexItem.IsMatch(acc.Username) || upregexItem.IsMatch(acc.Password))
                return 1; // Specific error message number

            // If the card info is not 16-digits long
            else if (!cardregex.IsMatch(acc.CardInfo))
                return 2;

            // If the CVV is not 3-digits long
            else if (!cvvregex.IsMatch(acc.Cvv))
                return 3;

            // If your edited username is already taken
            else if (CheckUsername(acc,1) == 1)
                return 4;

            // No problems found, go ahead and update the database with this information
            else
            {
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\caraz\source\repos\bingoLibrary\bingoLibrary\bingoAccounts.mdf;Integrated Security=True");
                sda = new SqlDataAdapter("UPDATE Accounts SET fname='" + acc.Fname + "', lname = '" + acc.Lname + "', username = '" + acc.Username + "', password = '" + acc.Password + "', cardInfo='" + acc.CardInfo + "', cvv='" + acc.Cvv + "', balance='" + acc.Balance + "' WHERE Id = " + acc.Id.ToString(), con);
                dt = new DataTable();
                sda.Fill(dt);

                scb = new SqlCommandBuilder(sda);
                sda.Update(dt);
                return 5; // Success number
            }
        }

        // Used to get the play displayed in Form2, "Current Play"
        public string GetPlay()
        {
            string play=""; // Used to keep track of what value will be returned
            int choice = 0; // Used to see what letter's number boundary we will look at
            Random r = new Random();

            // Generate a number 1 - 5
            choice = r.Next(1, 5); // (1=B, 2=I, 3=N, 4=G, 5=O)

            // Depending on choice's value, go generate a number in that letters boundaries

            if(choice == 1) // B
            {
                choice = r.Next(1, 15); // Generate a number 1-15
                if (choice < 10)
                    play = "B0" + choice; // If it is less than 10, add a 0 in between to make the string a legth of 3

                else
                    play = "B"+Convert.ToString(choice);
            }

            if (choice == 2) // I
            {
                choice = r.Next(16, 30); // Random number between 16-30
                play = "I" + Convert.ToString(choice);
            }

            if (choice == 3) // N
            {
                choice = r.Next(31, 45); // Random number between 31-45
                play = "N" + Convert.ToString(choice);
            }

            if (choice == 4) // G
            {
                choice = r.Next(46, 60); // Random number between 46-60
                play = "G" + Convert.ToString(choice);
            }

            if (choice == 5) // O
            {
                choice = r.Next(61, 75); // Random number between 61-75
                play = "O" + Convert.ToString(choice);
            }

            // By this point, play should have the format of "I25"
            return play;
        }

        // Generates the random values to fill the game board
        public string[] FillBoard()
        {
            Random r = new Random(); // Used to store the random number
            int size = 24; // Size of the array
            string[] arr = new string[size]; // Array that will store all the random values

            // Used for values in the B column
            for (int i = 0; i < 5; i++)
            {
                int use = r.Next(1, 15);
                // If value is less than 10, add a 0 at its beginning
                if(use < 10)
                {
                    string value = "0" + Convert.ToString(use);
                    arr[i] = value;
                }

                else
                    arr[i] = Convert.ToString(use);
            }

            // For column I
            for (int i = 5; i < 10; i++)
            {
                arr[i] = Convert.ToString(r.Next(16, 30));
            }

            // For column N
            for (int i = 10; i < 14; i++)
            {
                arr[i] = Convert.ToString(r.Next(31, 45));
            }

            // For column G
            for (int i = 14; i < 19; i++)
            {
                arr[i] = Convert.ToString(r.Next(46, 60));
            }

            // For column O
            for (int i = 19; i < 24; i++)
            {
                arr[i] = Convert.ToString(r.Next(61, 75));
            }

            return arr; // Returns the whole array
        }
    }
}
