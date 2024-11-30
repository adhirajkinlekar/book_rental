### Test Overview

- Almost all the topics mentioned in the test document have been covered, including the following features:

1. Search for Books by Name and/or Genre
2. Rent a Book
3. Return a Book
4. View Rental History
5. Mark Overdue Rentals
6. Email Notifications: Send an email notification to users when their rentals become overdue.
7. Stats: Display the most overdue book, most popular book, and least popular book.
8. Concurrency Handling

- Additionally, logging and global error handling have been implemented as required, along with authentication.

### Usage

1. Start the application, which will launch Swagger. Here is the list of API routes that you can use.
 
2. This is an example of an email that is sent to the user when the rental is overdue.

### Setup 

## Database 

1. Go to Scripts => Schema.sql and run the scripts to create the schema and populate the database

2. Go to appsettings.json and update the database connection as required.

3. Run the following scaffold command to generate the entities and db configuration.
-  dotnet ef dbcontext scaffold "Name=ConnectionStrings:BRDBConnection" Microsoft.EntityFrameworkCore.SqlServer --project=book_rental_api  --output-dir ./Data/book_rental_db
   
## Sendgrid

1. Create a sendgrid account

2. Create a token and register an email to send the email from

3. Use SendGrid in the EmailService (I wasn't able to implement a secure mechanism to store the API key as I am running out of time).