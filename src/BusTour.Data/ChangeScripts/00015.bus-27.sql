DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_beverage' and column_name = 'is_ticket' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour_beverage` ADD COLUMN `is_ticket` bit(1) NOT NULL DEFAULT 0;
	end if;

	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_beverage' and column_name = 'is_extra' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour_beverage` ADD COLUMN `is_extra` bit(1) NOT NULL DEFAULT 0;
	end if;

	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_menu' and column_name = 'is_ticket' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour_menu` ADD COLUMN `is_ticket` bit(1) NOT NULL DEFAULT 0;
	end if;

	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_menu' and column_name = 'is_extra' and table_schema = DATABASE()) 
	then
		ALTER TABLE `tour_menu` ADD COLUMN `is_extra` bit(1) NOT NULL DEFAULT 0;
	end if;
	
	CREATE TABLE IF NOT EXISTS `allergy` (
	  `id` tinyint NOT NULL AUTO_INCREMENT,
	  `name` json NOT NULL,
	  PRIMARY KEY (`id`)
	 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

	IF NOT EXISTS(SELECT 1
		FROM INFORMATION_SCHEMA.COLUMNS
		WHERE table_name = 'order_seat'
		AND column_name IN ('beverage_id','allergy_id','other_allergy') and table_schema = DATABASE()) 
	THEN
		ALTER TABLE `order_seat` ADD `beverage_id` int NULL;
		ALTER TABLE `order_seat` ADD `allergy_id` tinyint NULL;
		ALTER TABLE `order_seat` ADD `other_allergy` json NULL;
		ALTER TABLE `order_seat` ADD FOREIGN KEY order_seat_allergy (allergy_id) REFERENCES `allergy` (`id`);
		ALTER TABLE `order_seat` ADD FOREIGN KEY order_seat_beverage (beverage_id) REFERENCES `beverage` (`id`);	
	END IF;
	
	insert into allergy (id, name)
	values (1, json_object('en', 'Gluten', 'ru', 'Глютен')), 
		(2, json_object('en', 'Sugar', 'ru', 'Сахар')), 
		(3, json_object('en', 'Nuts', 'ru', 'Орехи')), 
		(4, json_object('en', 'Fish', 'ru', 'Рыба')), 
		(5, json_object('en', 'Other', 'ru', 'Другое'));
	
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;