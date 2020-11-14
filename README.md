# BettingBingoGame_Library
### This library is needed in order to use the the "BettingBingoGame" code found here: https://github.com/carathath/BettingBingoGame

This library's purpose is to serve as the connection to the database containing all the user information. Along with this, the library supplies the values that will fill the game board and the Current Play being shown. The database is made up of the following fields:
1. Id
1. username
1. password
1. fname
1. lname
1. cardInfo
1. cvv
1. balance

### Notice
In the functions: GetAccount, CheckUsername, AddAccount, UpdateBalance, and UpdateAccountInfo, you **MUST** change the connection string (Sqlconnection con) to match the one displayed on your machine. To find the connection string in Microsoft Visual Studio:
1. Locate the database on the *Solution Explorer*, titled **bingoAccounts.mdf**
1. Double-click the database and on the left-hand side, a *Server Explorer* tab should open
1. Right-click the **bingoAccounts.mdf** and on the bottom right there should appear a *Properties* window
1. Simply copy the string called *Connection String* onto the **Sqlconnection con** in the functions displayed above and things should work

The available functions in the library are:
* Setters/Getters for all fields found in the database
* ## GetAccount
  * From the **Form1** in the *BettingBingoGame*, it receives the user input for **username** and **password**
  * It queries through the database for this combination and returns an ojbect of the class with all the information
    * If there is no combination found, then it returns an object of the class with empty information
    * This is used as a trigger in **Form1** to display an error message
* ## CheckUsername
  * When the user decides to sign up for an account on **Form3** in the *BettingBingoGame* code, the username is queried in the database to check for a duplicate
  * Likewise, when the user decides to edit their username in **Form4** of the *BettingBingoGame*, it also gets queried through the database for duplicate
  * This is determined by a flag received when this function gets called, since the query command is different
    * 0 - When called from **Form3**
    * 1 - When called from **Form4**
  * If there is no match, then a 0 is returned. If there is a duplicate, a 1 is returned
* ## AddAccount
  * The user information that gets inputed in **Form3** goes through different regular expressions for validation
  * When a problem is found, a flag is returned to display an error message
  * If no regular expression is triggered, then the user gets added to the database
* ## UpdateBalance
  * Upon game completion, the user's balance gets updated in the database
    * All the necessary checking gets done on **Form2** before it gets sent here
* ## UpdateAccountInfo
  * Called when the user edits their account information on **Form4**
  * Has to go through regular expressions for validation, returning flags if something is wrong
  * Calls the *CheckUsername* function for username validation
  * If everything passes, the users record in the database gets updated
* ## CheckBingo
  * There is a counter for each row and column on the game board in **Form2**
  * As the user plays the game, the values are constantly checked in this function
  * When any row or column reached a value of 5, a flag is returned to end the game
* ## GetPlay
  * This function is used to return the next play to be displayed on the game board in **Form2**
  * First it generates a number, 1-5
    * These are used to represent a letter (1=B, 2=I, 3=N, 4=G, 5=O)
    * The letter gets added to a string
  * With that number, we then generate another number in the range of that letter
    * B:1-15, I:16-30, N:31-45, G:46-60, O:61-75
    * The number then gets concatenated with the first number generated to form a string
  * The string is then returned to be displayed on the board and checked for matches
* ## FillBoard
  * Used to return an array containing the values to be displayed on the board of **Form2**
  * In the 25-value array, index:
    * 0-4 are values for the B column
    * 5-9 are values for the I column
    * 10-14 are values for the N column
    * 15-19 are values for the G column
    * 20-24 are values for the O column
  * The array is returned to be displayed on **Form2**
