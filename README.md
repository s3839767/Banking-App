# s3719826-s3839767-a2

https://github.com/rmit-wdt-fs-2022/s3719826-s3839767-a2

Yonathan Kogan - s3719826
Mohammed Mousa - s3839767


Application Details:

My partner and I have developed an ASP.NET Core MVC Website for Internet Banking with an Azure Microsoft SQL Server database with access to the database being implemented using EF Core as the ORM.
The banking app website contains functional features of:
	- Logging in/out as a Customer

	- Depositing

	- Withdrawing

	- Transferring 

	- Confirmation Page for new transactions

	- My Statements page that contains a list of all transactions belonging to a specific account and split into a paged manner showing only 4 but allowing to move to next and previous pages

	- My Profiles page that contains details of customer's information and the ability to edit their details along with changing passwords

	- Bill Pay where bills can be created/modified or canceled by the customer and are paid in the background as the program runs or starts

All these features contain input checks to only allow the customer to input valid values into the website and are also ensured to follow all business rules required. 

Along with the banking app website we have developed an:

	- Admin API allows for the web to access Database data by exposing multiple endpoints.

	- Admin portal website able to let admin log in using pre set login values and be redirected to a main page where they can select to either select a user in which the modify, lock and transaction history features can be viewed or select billpay and view all current billpays.

The features also developed for the Admin section of app are:

	- Admin is able to view User Select page where they can do Modify Lock and Tranaction history pages.

	- Admin able to view transactions for each account within a specified start date and end date period entered but due to Account ID not properly passing through the Transaction history page isnt filled in with the transactions from the database.

	- Admin can Try to lock and unlock a user but due to API error with parsing it will not function properly.

	- Admin can Modify a users profile information other than their password via a Modify Profile button from the User Select page.

	- Admin can View BillPay entries in the database via Billpay page and is able to lock and unlock those billpays.


There have been also references of certain code where the MVC Login App from Tute 6, Background Service from Tute 9, MVC Movie from Lect 9 all created by the teaching staff of course, have been used as implemented.
