DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	-- TABLES
	
	UPDATE dinner_table
	SET 
		floor = 1,
		x_size = 1,
		y_size = 1,
		x = 496,
		y = 133,
		`number` = 1,
		table_category_id = 1
	WHERE id = 1;
	
	UPDATE dinner_table
	SET 
		floor = 1,
		x_size = 1,
		y_size = 1,
		x = 496,
		y = 8,
		`number` = 2,
		table_category_id = 1
	WHERE id = 2;
	
	UPDATE dinner_table
	SET 
		floor = 1,
		x_size = 2,
		y_size = 2,
		x = 590,
		y = 124,
		`number` = 3
	WHERE id = 3;
	
	UPDATE dinner_table
	SET 
		floor = 1,
		x_size = 2,
		y_size = 2,
		x = 590,
		y = 8,
		`number` = 4
	WHERE id = 4;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 74,
		y = 8,
		`number` = 5,
		is_left = 1,
		is_right = 0
	WHERE id = 5;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 74,
		y = 112,
		`number` = 6,
		is_left = 0,
		is_right = 1
	WHERE id = 6;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 2,
		y_size = 2,
		x = 162,
		y = 125,
		`number` = 7,
		is_left = 0,
		is_right = 1
	WHERE id = 7;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 2,
		y_size = 4,
		x = 162,
		y = 8,
		`number` = 8,
		is_left = 1,
		is_right = 0
	WHERE id = 8;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 859,
		y = 8,
		`number` = 9,
		is_left = 1,
		is_right = 0,
		table_category_id = 2
	WHERE id = 9;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 785,
		y = 8,
		`number` = 10,
		is_left = 1,
		is_right = 0
	WHERE id = 10;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 710,
		y = 8,
		`number` = 11,
		is_left = 1,
		is_right = 0
	WHERE id = 11;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 635,
		y = 8,
		`number` = 12,
		is_left = 1,
		is_right = 0
	WHERE id = 12;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 3,
		x = 560,
		y = 8,
		`number` = 13,
		is_left = 1,
		is_right = 0
	WHERE id = 13;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 859,
		y = 135,
		`number` = 14,
		is_left = 0,
		is_right = 1,
		table_category_id = 2
	WHERE id = 14;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 785,
		y = 135,
		`number` = 15,
		is_left = 0,
		is_right = 1
	WHERE id = 15;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 710,
		y = 135,
		`number` = 16,
		is_left = 0,
		is_right = 1,
		table_category_id = 1
	WHERE id = 16;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 635,
		y = 135,
		`number` = 17,
		is_left = 0,
		is_right = 1
	WHERE id = 17;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 560,
		y = 135,
		`number` = 18,
		is_left = 0,
		is_right = 1
	WHERE id = 18;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 485,
		y = 135,
		`number` = 19,
		is_left = 0,
		is_right = 1
	WHERE id = 19;
	
	UPDATE dinner_table
	SET 
		floor = 2,
		x_size = 1,
		y_size = 1,
		x = 410,
		y = 135,
		`number` = 20,
		is_left = 0,
		is_right = 1
	WHERE id = 20;
	
	INSERT INTO dinner_table (
		bus_id,
		floor,
		is_available,
		is_left,
		is_right,
		`number`,
		price,
		table_category_id,
		table_location_id,
		x,
		x_size,
		y,
		y_size
	)
	VALUES 
	(
		1,
		2,
		1,
		0,
		1,
		21,
		80,
		1,
		1,
		335,
		1,
		135,
		1
	);
	-- SEATS
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 1,
		name = 'A',
		x = 457,
		y = 124
	WHERE id = 1;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 2,
		name = 'A',
		x = 457,
		y = 7
	WHERE id = 2;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 3,
		name = 'A',
		x = 551,
		y = 124
	WHERE id = 3;
	
	UPDATE seat
	SET
		is_forward = 0,
		is_backward = 1,
		table_id = 3,
		name = 'B',
		x = 641,
		y = 124
	WHERE id = 4;
	
	UPDATE seat
	SET
		is_forward = 0,
		is_backward = 1,
		table_id = 4,
		name = 'B',
		x = 640,
		y = 7
	WHERE id = 5;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 4,
		name = 'A',
		x = 551,
		y = 7
	WHERE id = 6;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 5,
		name = 'A',
		x = 35,
		y = 8
	WHERE id = 7;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 5,
		name = 'B',
		x = 35,
		y = 43
	WHERE id = 8;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 6,
		name = 'B',
		x = 35,
		y = 125
	WHERE id = 9;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 6,
		name = 'A',
		x = 35,
		y = 90
	WHERE id = 10;
	
	UPDATE seat
	SET
		is_forward = 0,
		is_backward = 1,
		table_id = 7,
		name = 'B',
		x = 213,
		y = 125
	WHERE id = 11;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 7,
		name = 'A',
		x = 123,
		y = 125
	WHERE id = 12;
	
	UPDATE seat
	SET
		is_forward = 0,
		is_backward = 1,
		table_id = 8,
		name = 'B',
		x = 213,
		y = 8
	WHERE id = 13;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 8,
		name = 'A',
		x = 123,
		y = 8
	WHERE id = 14;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 8,
		name = 'C',
		x = 123,
		y = 43
	WHERE id = 15;
	
	UPDATE seat
	SET
		is_forward = 0,
		is_backward = 1,
		table_id = 8,
		name = 'D',
		x = 213,
		y = 43
	WHERE id = 16;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 9,
		name = 'A',
		x = 820,
		y = 8
	WHERE id = 17;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 9,
		name = 'B',
		x = 820,
		y = 43
	WHERE id = 18;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 10,
		name = 'A',
		x = 745,
		y = 8
	WHERE id = 19;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 10,
		name = 'B',
		x = 745,
		y = 43
	WHERE id = 20;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 11,
		name = 'A',
		x = 670,
		y = 8
	WHERE id = 21;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 11,
		name = 'B',
		x = 670,
		y = 43
	WHERE id = 22;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 12,
		name = 'A',
		x = 595,
		y = 8
	WHERE id = 23;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 12,
		name = 'B',
		x = 595,
		y = 43
	WHERE id = 24;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 13,
		name = 'A',
		x = 520,
		y = 8
	WHERE id = 25;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 13,
		name = 'B',
		x = 520,
		y = 43
	WHERE id = 26;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 14,
		name = 'A',
		x = 820,
		y = 125
	WHERE id = 27;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 15,
		name = 'A',
		x = 745,
		y = 125
	WHERE id = 28;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 16,
		name = 'A',
		x = 670,
		y = 125
	WHERE id = 29;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 17,
		name = 'A',
		x = 595,
		y = 125
	WHERE id = 30;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 18,
		name = 'A',
		x = 520,
		y = 125
	WHERE id = 31;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 19,
		name = 'A',
		x = 445,
		y = 125
	WHERE id = 32;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 20,
		name = 'A',
		x = 370,
		y = 125
	WHERE id = 33;
	
	UPDATE seat
	SET
		is_forward = 1,
		is_backward = 0,
		table_id = 21,
		name = 'A',
		x = 295,
		y = 125
	WHERE id = 34;
	
	DELETE
	FROM order_seat
	WHERE seat_id > 34;
	
	DELETE
	FROM seat
	WHERE id > 34;	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;