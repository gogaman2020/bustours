INSERT INTO [dbo].[Revision] (
    [CommonUserId]
    ,[CreatedOn])
VALUES 
    (@CommonUserId
    ,GETUTCDATE());

SELECT CAST(SCOPE_IDENTITY() as int);
