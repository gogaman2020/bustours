DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour' and column_name = 'type' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour` ADD COLUMN `type` tinyint NOT NULL DEFAULT 0;
	end if;	

	update bus
	set name = '{"en":"London BUS"}';

	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'bus' and column_name = 'name' and table_schema = DATABASE()) 
	then
		ALTER TABLE bus MODIFY COLUMN name json NOT NULL;
	end if;   

	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'client' and column_name = 'email' and table_schema = DATABASE()) 
	then
		ALTER TABLE client MODIFY COLUMN email varchar(256) NULL;
	end if; 

	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour' and column_name = 'route_id' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour MODIFY COLUMN route_id int NULL;
	end if;  
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_order' and column_name = 'is_group' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour_order` ADD COLUMN `is_group` tinyint(1) NOT NULL DEFAULT 0;
	end if;  

	CREATE TABLE IF NOT EXISTS `tour_private_hire` (
  		`id` int NOT NULL AUTO_INCREMENT,
  		`tour_id` int NOT NULL,
  		`duration` time NOT NULL,
  		`block_booking_date_from` datetime NOT NULL,
  		`block_booking_date_to` datetime NOT NULL,
  		`block_booking_for_draft` tinyint(1) NOT NULL,
  		`departure_point` varchar(512) NULL,
  		`arrival_point` varchar(512) NULL,
  		`stops` json NULL,
    	PRIMARY KEY (`id`),
  		CONSTRAINT `tour_private_hire_tour` FOREIGN KEY (`tour_id`) REFERENCES `tour` (`id`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;