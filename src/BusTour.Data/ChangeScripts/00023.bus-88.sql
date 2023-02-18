DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'gift_certificate' AND COLUMN_NAME = 'number' and TABLE_SCHEMA = DATABASE() 
	)
	then
		alter table gift_certificate modify `number` varchar(32) NOT NULL;
  	end if;  
  	

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;