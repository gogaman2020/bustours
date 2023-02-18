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
	( 1, 2, 1, 1,  1, 859, 135, 1, 1, 0, 1, 2, 80 ),
	( 1, 2, 1, 2,  1, 859, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1, 1, 3,  1, 785, 135, 1, 1, 0, 1, 2, 80 ),
	( 1, 1, 1, 4,  1, 785, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1, 1, 5,  1, 710, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1, 1, 6,  1, 710, 8,   1, 3, 1, 0, 2, 80 ),
	( 1, 1,	1, 7,  1, 635, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 8,  1, 635, 8,	1, 3, 1, 0,	2, 80 ),
	( 1, 1,	1, 9,  1, 560, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 10, 1, 560, 8,   1, 3, 1, 0,	2, 80 ),
	( 1, 1,	1, 11, 1, 485, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 12, 1, 410, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 13, 1, 335, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 14, 1, 260, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 15, 1, 183, 8,   2, 4, 1, 0,	2, 80 ),
	( 1, 1,	1, 16, 1, 185, 135,	1, 1, 0, 1,	2, 80 ),
	( 1, 1,	1, 17, 1, 74,  48,  2, 4, 0, 0,	2, 80 ),
	
	( 1, 1, 1, 18, 1, 590, 124, 2, 2, 0, 1, 1, 80 ),
 	( 1, 1, 1, 19, 1, 590, 8,   2, 2, 1, 0, 1, 80 ),
	( 1, 1, 2, 20, 1, 496, 133, 1, 1, 0, 1, 1, 80 ),
	( 1, 1, 2, 21, 1, 496, 8,   1, 1, 1, 0, 1, 80 );
	
	
	
	-- SEATS
	ALTER TABLE seat ADD COLUMN rotate INT NOT NULL;
	ALTER TABLE seat ADD COLUMN type INT NOT NULL;
	
	INSERT INTO seat (
		table_id,
		name,
		x,
		y,
		is_forward,
		is_backward,
		price,
		rotate,
		type
	)
	VALUES 
	( 1,  'A', 820, 125, 1, 0, 50, 0, 1 ),
	
	( 2,  'A', 820, 8,   1, 0, 50, 0, 1 ),
	( 2,  'B', 820, 43,  1, 0, 50, 0, 1 ),
	
	( 3,  'A', 745, 125, 1, 0, 50, 0, 1 ),
	
	( 4,  'A', 745, 8,   1, 0, 50, 0, 1 ),
	( 4,  'B', 745, 43,  1, 0, 50, 0, 1 ),
	
	( 5,  'A', 670, 125, 1, 0, 50, 0, 1 ),
	
	( 6,  'A', 670, 8,   1, 0, 50, 0, 1 ),
	( 6,  'B', 670, 43,  1, 0, 50, 0, 1 ),
	
	( 7,  'A', 595, 125, 1, 0, 50, 0, 1 ),
	
	( 8,  'A', 595, 8,   1, 0, 50, 0, 1 ),
	( 8,  'B', 595, 43,  1, 0, 50, 0, 1 ),
	
	( 9,  'A', 520, 125, 1, 0, 50, 0, 1 ),
	
	( 10, 'A', 520, 8,   1, 0, 50, 0, 1 ),
	( 10, 'B', 520, 43,  1, 0, 50, 0, 1 ),
	
	( 11, 'A', 445, 125, 1, 0, 50, 0, 1 ),
	( 12, 'A', 370, 125, 1, 0, 50, 0, 1 ),
	( 13, 'A', 295, 125, 1, 0, 50, 0, 1 ),
	( 14, 'A', 220, 125, 1, 0, 50, 0, 1 ),
	
	( 15, 'A', 142, 8,   1, 0, 50, 0, 1 ),
	( 15, 'B', 236, 8,   0, 1, 50, 2, 1 ),
	( 15, 'C', 142, 43,  1, 0, 50, 0, 1 ),
	( 15, 'D', 236, 43,  0, 1, 50, 2, 1 ),
	
	( 16, 'A', 145, 125, 1, 0, 50, 0, 1 ),
	
	( 17, 'A', 72, 8,    1, 0, 50, 2, 2 ),
	( 17, 'B', 35, 8,    1, 0, 50, 1, 3 ),
	( 17, 'C', 35, 44,   1, 0, 50, 0, 1 ),
	( 17, 'D', 35, 88,   1, 0, 50, 0, 1 ),
	( 17, 'E', 35, 125,  1, 0, 50, 0, 3 ),
	( 17, 'F', 72, 125,  1, 0, 50, 0, 2 ),
	
	( 18, 'A', 551, 124, 1, 0, 70, 0, 1 ),
	( 18, 'B', 641, 124, 0, 1, 70, 2, 1 ),
	
	( 19, 'B', 640, 7,   0, 1, 50, 2, 1 ),
	( 19, 'A', 551, 7,   1 ,0 ,50, 0, 1 ),
	
	( 20, 'A', 457, 124, 1, 0, 70, 0, 1 ),
	
	( 21, 'A', 457, 7,   1, 0, 70, 0, 1 );
	
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;