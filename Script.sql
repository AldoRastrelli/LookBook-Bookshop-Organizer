DROP DATABASE Library

USE Master

CREATE DATABASE Library;
GO

USE Library;
GO


CREATE TABLE Genre
(
ID int primary key identity(1,1),
GenreType varchar(100) not null unique
)

CREATE TABLE Publisher
(
ID int primary key identity(1,1),
PublisherName varchar(100) not null unique
)


CREATE TABLE Author
(
ID int primary key identity(1,1),
AuthorName varchar(100) not null unique,
)

CREATE TABLE Book
(	
ID int primary key identity(1,1),
Title varchar(100) not null,
ID_GenreType int not null,
ID_Publisher int not null,
ID_Author int not null,
CONSTRAINT FK_Genre FOREIGN KEY (ID_GenreType) REFERENCES Genre(ID),
CONSTRAINT FK_Publisher FOREIGN KEY (ID_Publisher) REFERENCES Publisher(ID),
CONSTRAINT FK_Author FOREIGN KEY (ID_Author) REFERENCES author(ID),
CONSTRAINT UQ_Book UNIQUE (Title, ID_Author)
)

SELECT * FROM Book

INSERT INTO Genre 
(GenreType)
VALUES 
('Cuento'),
('Novela'),
('Fábula'),
('Poesía'),
('Romance'),
('Drama'),
('Comedia'),
('Aventura'),
('Ciencia Ficción'),
('Policíaco'),
('Paranormal'),
('Distópico'),
('Fantástico'),
('Realismo Mágico')

INSERT INTO Genre 
(GenreType)
VALUES 
('Terror')

SELECT * FROM Genre


INSERT INTO Author
(AuthorName)
VALUES
('Gabriel García Márquez')

SELECT * FROM Author;
GO

SELECT * FROM Publisher
INSERT INTO Publisher
(PublisherName)
VALUES
('Random House')

INSERT INTO Book
(Title, ID_GenreType,ID_Publisher,ID_Author)
VALUES
('100 Años de Soledad',14,1,1)

INSERT INTO Publisher(PublisherName)VALUES('Minotauro')
INSERT INTO Author(AuthorName)VALUES('Ray Bradbury')

INSERT INTO Book
(Title, ID_GenreType,ID_Publisher,ID_Author)
VALUES
('El Hombre Ilustrado',14,2,2);

SELECT * FROM Book


SELECT 
	Book.Title,
	Author.AuthorName,
	Genre.GenreType,
	Publisher.PublisherName as [Publisher]

FROM
	Book

JOIN
	Genre
ON
	Genre.ID = Book.ID_GenreType
	
JOIN
	Publisher
ON
	Publisher.ID = Book.ID_Publisher
JOIN
	author
ON
	author.ID = Book.ID_Author
GO