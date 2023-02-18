DROP PROCEDURE IF EXISTS addCascadeDelete;

DELIMITER $$

CREATE PROCEDURE addCascadeDelete(
	IN tableName VARCHAR(255),
	IN refTableName VARCHAR(255),
	IN columnName VARCHAR(255),
	IN refColumnName VARCHAR(255)	
)
BEGIN
	
	DECLARE cnstr VARCHAR(255);
	
	SELECT CONSTRAINT_NAME INTO cnstr from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
	WHERE TABLE_NAME = tableName AND REFERENCED_TABLE_NAME = refTableName and 
    UNIQUE_CONSTRAINT_SCHEMA  = DATABASE() ;

	IF cnstr IS NOT NULL THEN
    
		SET @q = CONCAT('ALTER TABLE ', tableName, ' DROP FOREIGN KEY ', cnstr); 
	
		PREPARE stmt FROM @q; 
		EXECUTE stmt; 
		DEALLOCATE PREPARE stmt;
		
    END IF;	
   
   	SET @qq = CONCAT('ALTER TABLE ', tableName, ' ADD CONSTRAINT ', tableName, '_', refTableName ,' FOREIGN KEY (', columnName, ') REFERENCES ', refTableName, '(', refColumnName, ') ON DELETE CASCADE');
	
    PREPARE stmt FROM @qq;
	EXECUTE stmt; 
	DEALLOCATE PREPARE stmt;   
 
END$$

DELIMITER ;

CALL addCascadeDelete('tour_service_maintenance', 'tour', 'tour_id', 'id');
CALL addCascadeDelete('tour_private_hire', 'tour', 'tour_id', 'id');
CALL addCascadeDelete('tour_order', 'tour', 'tour_id', 'id');

DROP PROCEDURE IF EXISTS addCascadeDelete;

