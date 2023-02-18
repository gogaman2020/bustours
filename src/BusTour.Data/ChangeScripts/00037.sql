DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	DECLARE tableId INT(11);

	if not exists (
		select 1
		from seat s where s.`type`  = 5
	)
	then
		insert into dinner_table
		(bus_id, table_category_id, table_location_id, `number`, is_available, x, y, x_size, y_size, is_left, is_right, floor, price)
		VALUES(1, 1, 1, 0, 1, 440, 127, 1, 1, 0, 1, 1, 80.00);
	
		set tableId = LAST_INSERT_ID();
	
		INSERT INTO seat
		(table_id, name, x, y, is_forward, is_backward, price, rotate, `type`, scaleX, scaleY)
		VALUES(tableId, '', 430, 116, 0, 1, 70.00, 2, 5, 1, 1);
		
  	end if;  	
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;