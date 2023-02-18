DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'tour_private_hire' AND COLUMN_NAME = 'guest_count' and TABLE_SCHEMA = DATABASE() 
	)
	then
		alter table tour_private_hire add column guest_count int NOT NULL DEFAULT(0);
  	end if;  
  	

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;