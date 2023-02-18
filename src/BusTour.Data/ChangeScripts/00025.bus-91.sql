DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'notification' AND COLUMN_NAME = 'object_id' and TABLE_SCHEMA = DATABASE() 
	)
	then
		alter table notification add column object_id int DEFAULT NULL AFTER id;
  	end if;  

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;