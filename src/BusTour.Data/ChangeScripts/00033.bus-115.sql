DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	update dinner_table set table_location_id = 2 where id in (1,2);
	update dinner_table set table_location_id = 1 where id not in (1,2);	

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;





