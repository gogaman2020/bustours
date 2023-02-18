INSERT INTO public.audit (revisionid, entitytype, entityid, operation, data)
VALUES (@RevisionId, @EntityType, @EntityId, @Operation, CAST(@Data AS json)) returning id;