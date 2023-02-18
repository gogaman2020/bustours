DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	CREATE TABLE IF NOT EXISTS `tour_service_maintenance` (
  		`id` int NOT NULL AUTO_INCREMENT,
  		`tour_id` int NOT NULL,
  		`duration` time NOT NULL,
    	PRIMARY KEY (`id`),
  		CONSTRAINT `tour_service_maintenance_tour` FOREIGN KEY (`tour_id`) REFERENCES `tour` (`id`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;	
	
	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_order' and column_name = 'client_id' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour_order MODIFY COLUMN client_id int NULL;
	end if;  		
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;