DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	
	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'order_seat' and column_name = 'other_allergy' and table_schema = DATABASE()) 
	then
		ALTER TABLE order_seat MODIFY column other_allergy varchar(256) NULL;
	end if;  		
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;