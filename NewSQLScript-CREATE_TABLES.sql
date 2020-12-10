CREATE TABLE [dbo].[AcademicLevel]
 (
	academicLevelName NVARCHAR(50) NOT NULL PRIMARY KEY DEFAULT 'Not selected'
 );

CREATE TABLE [dbo].[Subject]
 (
	subjectName NVARCHAR(50) NOT NULL PRIMARY KEY DEFAULT 'Not selected'
 );

CREATE TABLE [dbo].[Assignment] 
 (
    [assignmentId]  INT IDENTITY (1, 1) NOT NULL,
	[title]         NVARCHAR(75)   NOT NULL,    
	[description]   NVARCHAR(1200)   NOT NULL,    
	[price]         INT CHECK (price > 0 AND price <10000) NOT NULL,    
	[postDate]      DATETIME       NOT NULL,    
	[deadline]      DATETIME       NOT NULL,    
	[anonymous]     BIT            NOT NULL,    
	[academicLevel] NVARCHAR(50)   NOT NULL,    
	[subject]       NVARCHAR(50)   NOT NULL,    
	[isActive]      BIT            NOT NULL,    
	[userId]        NVARCHAR (450) NOT NULL,    
	PRIMARY KEY CLUSTERED ([assignmentId] ASC),
    CONSTRAINT [fkAcademicLevel] FOREIGN KEY ([academicLevel]) REFERENCES [dbo].[AcademicLevel] ([academicLevelName]) ON DELETE SET DEFAULT ON UPDATE CASCADE,
    CONSTRAINT [fkSubject] FOREIGN KEY ([subject]) REFERENCES [dbo].[Subject] ([subjectName]) ON DELETE SET DEFAULT ON UPDATE CASCADE,
	CONSTRAINT [fkAuserId] foreign key(userId) references [Identity].[User](id),
);

CREATE TABLE [dbo].[AssignmentFile]
(
	assignmentId int NOT NULL,
	assignmentFile varbinary(max),
	CONSTRAINT [fkFileAssignmentId] foreign key(assignmentId) references [Assignment](assignmentId),
);

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
	 CONSTRAINT [fkSassignmentId] foreign key(assignmentId) references [Assignment](assignmentId),
	 CONSTRAINT [fkSuserId] foreign key(userId) references [Identity].[User](id),
 );

CREATE TABLE [dbo].[SolutionFile] (
	solutionId int NOT NULL,
	solutionFile varbinary(max),
	CONSTRAINT [fkFileSolutionId] foreign key(solutionId) references [Solution](solutionId),
)

CREATE TABLE [dbo].[Customer]

 (
   	 [userId] NVARCHAR(450) NOT NULL,

     [title] NVARCHAR(50) NOT NULL, 

     [rating] FLOAT NOT NULL,  

     [credit] FLOAT NOT NULL,
     constraint fkCuserId foreign key(userId) references [Identity].[User](Id) ON DELETE CASCADE ON UPDATE CASCADE,

 )