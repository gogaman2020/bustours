INSERT INTO `%TableName%`
    (%Fields%)
VALUES
    (%FieldValues%)
ON DUPLICATE KEY UPDATE
    %UpdateFieldValues%
;

SELECT LAST_INSERT_ID();