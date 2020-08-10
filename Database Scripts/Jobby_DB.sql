CREATE DATABASE Jobby;

CREATE TABLE Roles(
ID uniqueidentifier Not NULL DEFAULT NEWID() PRIMARY KEY,
Name nvarchar(50) Not NULL
);

CREATE TABLE Users(
ID uniqueidentifier Not NULL DEFAULT NEWID() PRIMARY KEY,
FName nvarchar(50) Not NULL,
LName nvarchar(50) Not NULL,
Img varbinary(MAX),
Email varchar(320) Not NULL UNIQUE,
PW nvarchar(MAX) Not NULL,
Code uniqueidentifier Not NULL,
IsVerified bit Not NULL default 0,
AccessFaildCount tinyint Not NULL default 0,
LockoutEndDate Datetime2
);

CREATE TABLE UserRoles(
UserID uniqueidentifier Not NULL  FOREIGN KEY REFERENCES Users (ID),
RoleID uniqueidentifier Not NULL  FOREIGN KEY REFERENCES Roles (ID),
Notes nvarchar(50),
CONSTRAINT PK_UR PRIMARY KEY (UserID,RoleID)
);

CREATE TABLE Countries (
  Id  int IDENTITY(1,1) NOT NULL,
  Iso  varchar(2) NOT NULL,
  Name  varchar(80) NOT NULL,
  Iso3  varchar(3) NULL,
  NumCode int  NULL,
  PhoneCode int NOT NULL,
  CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([Id] ASC),
  CONSTRAINT [uc_Countries_Iso] UNIQUE NONCLUSTERED ([Iso] ASC)
);

CREATE TABLE Employees(
UserID uniqueidentifier Not NULL PRIMARY KEY FOREIGN KEY REFERENCES Users (ID),
BirthDate date,
CountryID int FOREIGN KEY REFERENCES Countries (Id),
City nvarchar(50),
PhoneNumber varchar(20),
Salary int,
JobStatus nvarchar(50),
MilitaryStatus nvarchar(20),
MartialStatus nvarchar(20),
CareerLevel nvarchar(30),
CV varbinary(MAX)
);

CREATE TABLE Employers(
UserID uniqueidentifier Not NULL PRIMARY KEY FOREIGN KEY REFERENCES Users (ID),
CompanyName nvarchar(100) Not NULL,
Indusrty nvarchar(50) Not NULL,
Website varchar(MAX),
);

CREATE TABLE UserSkills(
ID uniqueidentifier Not NULL DEFAULT NEWID() PRIMARY KEY,
UserID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Users (ID),
SkillName nvarchar(50) Not NULL,
SkillLevel tinyint Not NULL,
IsLanguage bit Not NULL default 0
);

CREATE TABLE UserEducation(
UserID uniqueidentifier Not NULL PRIMARY KEY FOREIGN KEY REFERENCES Users (ID),
Degree nvarchar(50) Not NULL,
Entity nvarchar(100) Not NULL,
Field nvarchar(100) Not NULL,
Grade nvarchar(50) Not NULL,
StartYear char(4) Not NULL,
EndYear char(4) Not NULL
);

CREATE TABLE JobFields(
ID int Not NULL IDENTITY(1,1) PRIMARY KEY,
Name nvarchar(100) Not NULL
);

CREATE TABLE Jobs(
ID uniqueidentifier Not NULL DEFAULT NEWID() PRIMARY KEY,
UserID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Users (ID),
Title nvarchar(50) Not NULL,
JobDesc nvarchar(MAX) Not NULL,
PostDate datetime2 Not NULL,
CountryID int Not NULL FOREIGN KEY REFERENCES Countries (Id),
City nvarchar(50) Not NULL,
Salary int Not NULL,
JobStatus char(1) Not NULL DEFAULT 'o',
CareerLevel nvarchar(30) Not NULL,
JobFieldID int Not NULL FOREIGN KEY REFERENCES JobFields (ID)
);

CREATE TABLE Applications(
UserID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Users (ID),
JobID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Jobs (ID),
ApplicationStatus nvarchar(11) Not NULL DEFAULT 'pending',
CONSTRAINT PK_UA PRIMARY KEY (UserID,JobID)
);

CREATE TABLE SavedJobs(
UserID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Users (ID),
JobID uniqueidentifier Not NULL FOREIGN KEY REFERENCES Jobs (ID),
Notes nvarchar(50),
CONSTRAINT PK_US PRIMARY KEY (UserID,JobID)
);