CREATE DATABASE dbSite;

USE dbSite;
GO

CREATE TABLE User(
	ID int PRIMARY KEY AUTO_INCREMENT ,
	UserName varchar(30) ,
	Email varchar(30), 
	Pwd varchar(30)); 

CREATE TABLE Post(
	ID int PRIMARY KEY AUTO_INCREMENT,
	Link varchar(100) ,
	Description varchar(200) ,
	PublicationDate datetime ,
	Upvote int,
	DownVote int,
	UserID int,
	FOREIGN KEY(UserID) REFERENCES User(ID));

CREATE TABLE Comment(
	ID int PRIMARY KEY AUTO_INCREMENT,
	Description varchar(200) ,
	PublicationDate datetime ,
	UserID int ,
	FOREIGN KEY(UserID) REFERENCES User(ID),
	PostID int ,
	FOREIGN KEY(PostID) REFERENCES Post(ID));


INSERT INTO User(UserName, Email, Pwd) VALUES ('Safae', 'safae@gmail.com', '123');
INSERT INTO User(UserName, Email, Pwd) VALUES ('Noura', 'noura@gmail.com', '123');
INSERT INTO User(UserName, Email, Pwd) VALUES ('tatiana', 'tatiana@gmail.com', '123');