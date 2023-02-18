DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'gift_certificate' and column_name = 'redeemed_amount' and table_schema = DATABASE()) 
	then
		ALTER TABLE `gift_certificate` ADD COLUMN `redeemed_amount` decimal(20,2) NULL;
	end if;
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'gift_certificate' and column_name = 'redeemed_date' and table_schema = DATABASE()) 
	then
		ALTER TABLE `gift_certificate` ADD COLUMN `redeemed_date` datetime NULL;
	end if;
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'gift_certificate' and column_name = 'cancelled' and table_schema = DATABASE()) 
	then
		ALTER TABLE `gift_certificate` ADD COLUMN `cancelled` tinyint(1) NOT NULL DEFAULT '0';
	end if;	
	
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;