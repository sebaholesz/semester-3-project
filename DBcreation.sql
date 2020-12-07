 CREATE TABLE [dbo].[Moderator]

 (    
 
	  [userId] NVARCHAR(450) NOT NULL,

      [levelOfPriviliges] NVARCHAR(50) NOT NULL,

	  constraint fkMuserId foreign key(userId) references [User](userId),

 )
 
CREATE TABLE [dbo].[Customer]

 (

   	 [userId] NVARCHAR(450) NOT NULL,

     [title] NVARCHAR(50) NOT NULL, 

     [rating] FLOAT NOT NULL, 

     [facebookAccountUrl] NVARCHAR(150) NOT NULL, 

     [credit] FLOAT NOT NULL

     constraint fkCuserId foreign key(userId) references [User](userId),

 )
 

 CREATE TABLE [dbo].[Watchlist]

 (
	 [userId] NVARCHAR(450) NOT NULL,

     [watchlistType] NVARCHAR(50) NOT NULL, 

	 [noOfItems] INT NOT NULL, 

	 constraint fkWuserId foreign key(userId) references [User](userId),

 )
 
 CREATE TABLE [dbo].[Interests]

 (

 	 [userId] NVARCHAR(450) NOT NULL,

     [interest] NVARCHAR(50) NOT NULL,

	 constraint fkCIcustomerId foreign key(userId) references [User](userId),

 )

  CREATE TABLE [dbo].[Education]

 (

  	 [userId] NVARCHAR(450) NOT NULL,

     [educationProgram] NVARCHAR(50) NOT NULL,

	 [institution] NVARCHAR(50) NOT NULL,

	 [duration] NVARCHAR(50) NOT NULL,

	 constraint fkCEcustomerId foreign key(userId) references [User](userId),
 )

CREATE TABLE [dbo].[Assignment] (

    [assignmentId]  INT            IDENTITY (1, 1) NOT NULL,
    
	[title]         NVARCHAR(75)   NOT NULL,
    
	[description]   NVARCHAR(1200)   NOT NULL,
    
	[price]         INT CHECK (price > 0 AND price <10000) NOT NULL,
    
	[postDate]      DATETIME       NOT NULL,
    
	[deadline]      DATETIME       NOT NULL,
    
	[anonymous]     BIT            NOT NULL,
    
	[academicLevel] NVARCHAR(50)   DEFAULT ('None') NOT NULL,
    
	[subject]       NVARCHAR(50)   DEFAULT ('None') NOT NULL,
    
	[isActive]      BIT            NOT NULL,
    
	[userId]        NVARCHAR (450) NOT NULL,
    
	PRIMARY KEY CLUSTERED ([assignmentId] ASC),
    CONSTRAINT [fkAcademicLevel] FOREIGN KEY ([academicLevel]) REFERENCES [dbo].[AcademicLevel] ([academicLevelName]) ON DELETE SET DEFAULT ON UPDATE CASCADE,
    CONSTRAINT [fkSubject] FOREIGN KEY ([subject]) REFERENCES [dbo].[Subject] ([subjectName]) ON DELETE SET DEFAULT ON UPDATE CASCADE
);

   CREATE TABLE [dbo].[StickyNote]

 (

     [assignmentId] INT NOT NULL,

     [text] NVARCHAR(50) NOT NULL,

	 constraint fkSNassignmentId foreign key(assignmentId) references [Assignment](assignmentId),
 )

    CREATE TABLE [dbo].[AssignmentComment]

 (

     [assignmentId] INT NOT NULL ,

     [text] NVARCHAR(50) NOT NULL,

	 constraint fkACassignmentId foreign key(assignmentId) references [Assignment](assignmentId),
 )

   CREATE TABLE [dbo].[Solution]

 (

     [solutionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),

     [assignmentId] INT NOT NULL,

 	 [userId] NVARCHAR(450) NOT NULL,

     [description] NVARCHAR(1200) NOT NULL,

     [timestamp] DATETIME NOT NULL,

	 [solutionRating] DECIMAL(2,1) NOT NULL,

	 [anonymous] BIT NOT NULL,
	 
	 [accepted] BIT NOT NULL,


	 constraint fkSassignmentId foreign key(assignmentId) references [Assignment](assignmentId),
	 constraint fkSuserId foreign key(userId) references [Identity].[User](id),
 )

   CREATE TABLE [dbo].[Order]

 (

     [orderId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),

     [solutionId] INT NOT NULL,

     [timestamp] TIMESTAMP NOT NULL,

	 constraint fkOsolutionId foreign key(solutionId) references [Solution](solutionId),

 )

   CREATE TABLE [dbo].[Forum]

 (

     [forumId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),


 )

  CREATE TABLE [dbo].[Question]

 (

     [questionId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),

     [forumId] INT NOT NULL,

     [title] NVARCHAR(50) NOT NULL,

 	 [userId] NVARCHAR(450) NOT NULL,

     [anonymous] BIT NOT NULL,

	 constraint fkQforumId foreign key(forumId) references [Forum](forumId),
	 constraint fkQuserId foreign key(userId) references [Identity].[User](id),
 )

CREATE TABLE [dbo].[Comment]

 (

     [commentId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),

     [forumId] INT NOT NULL,

 	 [userId] NVARCHAR(450) NOT NULL,

     [text] NVARCHAR(50) NOT NULL,

     [anonymous] BIT NOT NULL,

	 constraint fkCforumId foreign key(forumId) references [Forum](forumId),
	 constraint fkCuserId foreign key(userId) references [Identity].[User](id),
 )

CREATE TABLE [dbo].[AcademicLevel]
 (
	academicLevelName NVARCHAR(50) NOT NULL PRIMARY KEY
 )

CREATE TABLE [dbo].[Subjects]
 (
	subjectName NVARCHAR(50) NOT NULL PRIMARY KEY
 )

create table [dbo].[AssignmentFile] (
	assignmentId int NOT NULL,
	assignmentFile varbinary(max),
	constraint fkFileAssignmentId foreign key(assignmentId) references [Assignment](assignmentId),
)

create table [dbo].[SolutionFile] (
	solutionId int NOT NULL,
	solutionFile varbinary(max),
	constraint fkFileSolutionId foreign key(solutionId) references [Solution](solutionId),
)










 