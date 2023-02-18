DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	DELETE FROM order_seat;
	DELETE FROM seat;
	DELETE FROM dinner_table;
	DELETE FROM order_beverage;
	DELETE FROM order_menu;
	DELETE FROM payment;
	DELETE from order_surprise;
	DELETE FROM tour_order;
	
	
	ALTER TABLE order_seat  AUTO_INCREMENT = 1;
	ALTER TABLE seat  AUTO_INCREMENT = 1;
	ALTER TABLE dinner_table  AUTO_INCREMENT = 1;
	ALTER TABLE order_beverage  AUTO_INCREMENT = 1;
	ALTER TABLE order_menu  AUTO_INCREMENT = 1;
	ALTER TABLE payment  AUTO_INCREMENT = 1;
	ALTER TABLE order_surprise  AUTO_INCREMENT = 1;
	ALTER TABLE tour_order  AUTO_INCREMENT = 1;

	-- TABLES	
	INSERT INTO dinner_table (
		bus_id,
		table_category_id,
		table_location_id,
		`number`,
		is_available,
		x,
		y,
		x_size,
		y_size,
		is_left,
		is_right,
		floor,
		price
	)
	VALUES 
    ( 1, 2, 1, 1,  1, 863, 129, 1, 1, 0, 1, 2, 80 ),
	( 1, 2, 1, 2,  1, 863, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1, 1, 3,  1, 789, 129, 1, 1, 0, 1, 2, 80 ),
	( 1, 1, 1, 4,  1, 789, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1, 1, 5,  1, 714, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1, 1, 6,  1, 714, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1,	1, 7,  1, 639, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 8,  1, 639, 8,	1, 3, 1, 0,	2, 80 ),
	( 1, 1,	1, 9,  1, 564, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 10, 1, 564, 8,   1, 3, 1, 0,	2, 80 ),
	( 1, 1,	1, 11, 1, 489, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 12, 1, 414, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 13, 1, 339, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 14, 1, 264, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 15, 1, 189, 8,   1, 3, 1, 0,	2, 80 ),
	( 1, 1,	1, 16, 1, 189, 129,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 17, 1, 78,  48,  1, 4, 0, 0,	2, 80 ),
	
	( 1, 1, 1, 18, 1, 557, 127, 1, 1, 0, 1, 1, 80 ),
 	( 1, 1, 1, 19, 1, 557, 8,   1, 1, 1, 0, 1, 80 );
	
	
	
	-- SEATS
 	ALTER TABLE seat ADD COLUMN scaleX INT NOT NULL;
	ALTER TABLE seat ADD COLUMN scaleY INT NOT NULL;

	INSERT INTO seat (
		table_id,
		name,
		x,
		y,
		is_forward,
		is_backward,
		price,
		rotate,
		type,
		scaleX,
		scaleY
	)
	VALUES 
	( 1,  'A', 820, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 2,  'A', 820, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 2,  'B', 820, 36,  1, 0, 50, 0, 4, 1, -1 ),

	( 3,  'A', 745, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 4,  'A', 745, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 4,  'B', 745, 36,  1, 0, 50, 0, 4, 1, -1 ),

	( 5,  'A', 670, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 6,  'A', 670, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 6,  'B', 670, 36,  1, 0, 50, 0, 4, 1, -1 ),

	( 7,  'A', 595, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 8,  'A', 595, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 8,  'B', 595, 36,  1, 0, 50, 0, 4, 1, -1 ),

	( 9,  'A', 520, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 10, 'A', 520, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 10, 'B', 520, 36,  1, 0, 50, 0, 4, 1, -1 ),

	( 11, 'A', 445, 118, 1, 0, 50, 0, 1, 1, 1 ),
	( 12, 'A', 370, 118, 1, 0, 50, 0, 1, 1, 1 ),
	( 13, 'A', 295, 118, 1, 0, 50, 0, 1, 1, 1 ),
	( 14, 'A', 220, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 15, 'A', 145, 3,   1, 0, 50, 0, 4, 1, 1 ),
	( 15, 'B', 220, 3,   0, 1, 50, 2, 4, -1, 1 ),
	( 15, 'C', 145, 36,  1, 0, 50, 0, 4, 1, -1 ),
	( 15, 'D', 220, 36,  0, 1, 50, 2, 4, -1, -1 ),

	( 16, 'A', 145, 118, 1, 0, 50, 0, 1, 1, 1 ),

	( 17, 'A', 69, 8,    1, 0, 50, 0, 2, 1, 1 ),
	( 17, 'D', 69, 123,  1, 0, 50, 0, 2, 1, -1 ),
	( 17, 'B', 35, 7,    1, 0, 50, 0, 3, 1, 1 ),
	( 17, 'C', 35, 81,   1, 0, 50, 0, 3, 1, -1 ),

	( 18, 'A', 513, 116, 1, 0, 70, 0, 1, 1, 1 ),
	( 18, 'B', 589, 116, 0, 1, 70, 2, 1, -1, 1 ),

	( 19, 'B', 589, 7,   0, 1, 50, 2, 1, -1, 1 ),
	( 19, 'A', 513, 7,   1 ,0 ,50, 0, 1, 1, 1 );
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;