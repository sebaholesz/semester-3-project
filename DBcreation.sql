CREATE TABLE [dbo].[User]















 
CREATE TABLE [dbo].[Customer]








 


	 [userId] INT NOT NULL,  


	 [noOfItems] INT NOT NULL, 

	 constraint fkWuserId foreign key(userId) references [User](userId),

 
 CREATE TABLE [dbo].[Interests]




	 constraint fkCIcustomerId foreign key(userId) references [User](userId),





	 [institution] VARCHAR(50) NOT NULL,












   CREATE TABLE [dbo].[StickyNote]





    CREATE TABLE [dbo].[AssignmentComment]





   CREATE TABLE [dbo].[Solution]







	 [solutionRating] FLOAT NOT NULL,

	 [anonymous] BIT NOT NULL,

	 constraint fkSassignmentId foreign key(assignmentId) references [Assignment](assignmentId),

   CREATE TABLE [dbo].[Order]





	 constraint fkOsolutionId foreign key(solutionId) references [Solution](solutionId),


   CREATE TABLE [dbo].[Forum]





  CREATE TABLE [dbo].[Question]







	 constraint fkQforumId foreign key(forumId) references [Forum](forumId),

CREATE TABLE [dbo].[Comment]







	 constraint fkCforumId foreign key(forumId) references [Forum](forumId),


