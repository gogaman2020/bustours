DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'promo_code' and column_name = 'series_number' and table_schema = DATABASE()) 
	then
		ALTER TABLE `promo_code` ADD COLUMN `series_number` VARCHAR(200) DEFAULT NULL;
	end if;  
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'promo_code' and column_name = 'create_date' and table_schema = DATABASE()) 
	then
		ALTER TABLE `promo_code` ADD COLUMN `create_date` DATETIME DEFAULT NULL;
	end if;  
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;