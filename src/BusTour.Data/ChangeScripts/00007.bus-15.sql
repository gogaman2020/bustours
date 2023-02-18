DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	CREATE TABLE IF NOT EXISTS `tour_menu` (
	  `id` int NOT NULL AUTO_INCREMENT,
	  `tour_id` int NOT NULL,
	  `menu_id` tinyint NOT NULL,
	  PRIMARY KEY (`id`),
	  KEY `fk_tour_menu_tour` (`tour_id`),
	  KEY `fk_tour_menu_menu` (`menu_id`),
	  CONSTRAINT `tour_menu_ibfk_1` FOREIGN KEY (`tour_id`) REFERENCES `tour` (`id`),
	  CONSTRAINT `tour_menu_ibfk_2` FOREIGN KEY (`menu_id`) REFERENCES `menu` (`id`)
	) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

	CREATE TABLE IF NOT EXISTS `tour_beverage` (
	  `id` int NOT NULL AUTO_INCREMENT,
	  `tour_id` int NOT NULL,
	  `beverage_id` smallint NOT NULL,
	  PRIMARY KEY (`id`),
	  KEY `fk_tour_beverage_tour` (`tour_id`),
	  KEY `fk_tour_beverage_beverage` (`beverage_id`),
	  CONSTRAINT `tour_beverage_ibfk_1` FOREIGN KEY (`tour_id`) REFERENCES `tour` (`id`),
	  CONSTRAINT `tour_beverage_ibfk_2` FOREIGN KEY (`beverage_id`) REFERENCES `beverage` (`id`)
	) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

	IF NOT EXISTS(SELECT 1
		FROM INFORMATION_SCHEMA.COLUMNS
		WHERE table_name = 'tour'
		AND column_name IN ('seat_price','vip_price','type') and 
		table_schema = DATABASE()) 
	THEN
		ALTER TABLE `tour` ADD `seat_price` decimal(20,2) NULL;
		ALTER TABLE `tour` ADD `vip_price` decimal(20,2) NULL;
		ALTER TABLE `tour` ADD `type` tinyint NULL;
	END IF;
	
	ALTER TABLE `tour_menu` ADD CONSTRAINT `tour_menu_uq_1` UNIQUE (`tour_id`, `menu_id`);
	
	ALTER TABLE `tour_beverage` ADD CONSTRAINT `tour_menu_uq_1` UNIQUE (`tour_id`, `beverage_id`);	

END$$
DELIMITER ;
CALL migration();

DROP PROCEDURE IF EXISTS migration;