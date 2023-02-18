INSERT INTO public.revision (commonuserid, createdon)
VALUES (@CommonUserId, now()) returning id;