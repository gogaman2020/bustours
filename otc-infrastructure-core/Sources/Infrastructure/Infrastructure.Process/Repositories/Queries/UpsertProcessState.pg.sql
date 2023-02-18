INSERT INTO public.%TableName%
    (%Fields%)
VALUES
    (%FieldValues%)
ON CONFLICT (id) 
DO UPDATE
    %UpdateFieldValues%
returning id;