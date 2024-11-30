-- Create the database and then run the following queries all togeather

CREATE DATABASE book_rental_db;

USE book_rental_db;

-- --------------------------------------------------------------------------------------------------------------------------------------------
 
CREATE TABLE authors (
   id INT IDENTITY(1, 1) PRIMARY KEY,
   full_name VARCHAR(100) UNIQUE NOT NULL -- I would normally separate the first and last names but I am using the full name for simplicity.
);
 
CREATE TABLE genres (
    id INT IDENTITY(1, 1) PRIMARY KEY,  -- Auto-incremented genre ID
    name VARCHAR(100) UNIQUE NOT NULL  -- Unique genre name
);
 
CREATE TABLE books (
    id INT IDENTITY(1, 1) PRIMARY KEY,
    title VARCHAR(100) UNIQUE NOT NULL,
    isbn VARCHAR(13),
    genre_id INT NOT NULL,  -- References the 'genres' table
    author_id INT NOT NULL, -- Implemented a one-to-one relationship between the books and orders tables, considering the sample data.
    CONSTRAINT FK_Books_Authors FOREIGN KEY (author_id) 
    REFERENCES authors(id)
    ON DELETE NO ACTION   
    ON UPDATE NO ACTION,
    CONSTRAINT FK_Books_Genres FOREIGN KEY (genre_id) 
    REFERENCES genres(id)
    ON DELETE NO ACTION 
    ON UPDATE NO ACTION
);

CREATE TABLE users (
    id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    PasswordHash VARBINARY(MAX) NOT NULL,
    PasswordSalt VARBINARY(MAX) NOT NULL
);

CREATE TABLE rental_records (
   id INT IDENTITY(1, 1) PRIMARY KEY,
   book_id INT NOT NULL, 
   user_id INT NOT NULL,
   rented_on DATETIME  NOT NULL,
   returned_on DATETIME,  
   is_overdue BIT NOT NULL DEFAULT 0, 
   CONSTRAINT FK_RentalHistory_Books FOREIGN KEY (book_id) 
      REFERENCES books(id) 
      ON DELETE NO ACTION   
      ON UPDATE NO ACTION,
   CONSTRAINT FK_RentalHistory_Users FOREIGN KEY (user_id) 
      REFERENCES users(id) 
      ON DELETE NO ACTION   
      ON UPDATE NO ACTION
);

INSERT INTO authors (full_name) VALUES
('F. Scott Fitzgerald'),
('Harper Lee'),
('George Orwell'),
('Jane Austen'),
('J.D. Salinger'),
('J.R.R. Tolkien'),
('Ray Bradbury'),
('Markus Zusak'),
('Herman Melville'),
('Leo Tolstoy');
 
INSERT INTO genres (name) VALUES
('Classics'),
('Dystopian'),
('Romance'),
('Fantasy'),
('Science Fiction'),
('Historical Fiction');

INSERT INTO books (title, isbn, genre_id, author_id) VALUES
('The Great Gatsby', '9780743273565', 1, 1),
('To Kill a Mockingbird', '9780060935467', 1, 2),
('1984', '9780451524935', 2, 3),
('Pride and Prejudice', '9780141199078', 3, 4),
('The Catcher in the Rye', '9780316769488', 1, 5),
('The Hobbit', '9780547928227', 4, 6),
('Fahrenheit 451', '9781451673319', 5, 7),
('The Book Thief', '9780375842207', 6, 8),
('Moby-Dick', '9781503280786', 1, 9),
('War and Peace', '9781400079988', 6, 10);