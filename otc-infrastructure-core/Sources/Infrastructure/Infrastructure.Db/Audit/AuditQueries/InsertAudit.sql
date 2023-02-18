INSERT INTO [dbo].[Audit] ([RevisionId], [EntityType], [EntityId], [Operation], [Data])
VALUES (@RevisionId, @EntityType, @EntityId, @Operation, @Data);

SELECT CAST(SCOPE_IDENTITY() as int);
