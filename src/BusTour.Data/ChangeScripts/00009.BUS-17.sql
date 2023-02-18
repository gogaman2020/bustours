DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	IF EXISTS(SELECT 1
		FROM INFORMATION_SCHEMA.COLUMNS
		WHERE column_name = 'id'
		AND data_type != 'int'
		AND table_name IN ('beverage') and
		table_schema = DATABASE()
	) 
	THEN
		ALTER TABLE route DROP FOREIGN KEY `route_ibfk_1`;
		ALTER TABLE route DROP FOREIGN KEY `route_ibfk_2`;
		ALTER TABLE order_beverage DROP FOREIGN KEY `order_beverage_ibfk_2`;
		ALTER TABLE wine DROP FOREIGN KEY `wine_ibfk_1`;
		ALTER TABLE tour_beverage DROP FOREIGN KEY `tour_beverage_ibfk_2`;
		ALTER TABLE beverage DROP FOREIGN KEY `beverage_ibfk_1`;
		ALTER TABLE address DROP FOREIGN KEY `address_ibfk_1`;
		ALTER TABLE gift_certificate DROP FOREIGN KEY `gift_certificate_ibfk_1`;
		ALTER TABLE wine DROP FOREIGN KEY `wine_ibfk_2`;
		ALTER TABLE dinner_table DROP FOREIGN KEY `dinner_table_ibfk_2`;
		ALTER TABLE dinner_table DROP FOREIGN KEY `dinner_table_ibfk_1`;
		ALTER TABLE gift_certificate DROP FOREIGN KEY `gift_certificate_ibfk_3`;
		ALTER TABLE order_surprise DROP FOREIGN KEY `order_surprise_ibfk_2`;
		ALTER TABLE gift_certificate DROP FOREIGN KEY `gift_certificate_ibfk_2`;
		ALTER TABLE order_seat DROP FOREIGN KEY `order_seat_ibfk_2`;
		ALTER TABLE tour DROP FOREIGN KEY `tour_ibfk_1`;
		ALTER TABLE route_image DROP FOREIGN KEY `route_image_ibfk_1`;
		ALTER TABLE tour_order DROP FOREIGN KEY `tour_order_ibfk_1`;
		ALTER TABLE menu DROP FOREIGN KEY `menu_ibfk_1`;
		ALTER TABLE order_menu DROP FOREIGN KEY `order_menu_ibfk_2`;
		ALTER TABLE order_seat DROP FOREIGN KEY `order_seat_ibfk_3`;
		ALTER TABLE tour_menu DROP FOREIGN KEY `tour_menu_ibfk_2`;
		ALTER TABLE seat DROP FOREIGN KEY `seat_ibfk_1`;
		ALTER TABLE tour_order DROP FOREIGN KEY `tour_order_ibfk_4`;
		ALTER TABLE city DROP FOREIGN KEY `city_ibfk_1`;
		ALTER TABLE `route` DROP FOREIGN KEY `route_ibfk_3`;
		ALTER TABLE `route` DROP FOREIGN KEY `route_ibfk_4`;
		ALTER TABLE `surprise` DROP FOREIGN KEY `surprise_ibfk_1`;
		ALTER TABLE `beverage` DROP FOREIGN KEY `beverage_ibfk_2`;
		ALTER TABLE `route_image` DROP FOREIGN KEY `route_image_ibfk_2`;
		ALTER TABLE `menu` DROP FOREIGN KEY `menu_ibfk_2`;

		ALTER TABLE address MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE route MODIFY departure_address_id int NOT NULL;		
		ALTER TABLE beverage MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE order_beverage MODIFY beverage_id int NOT NULL;
		ALTER TABLE beverage MODIFY group_id int NOT NULL;
		ALTER TABLE wine MODIFY id int NOT NULL;
		ALTER TABLE tour_beverage MODIFY beverage_id int NOT NULL;
		ALTER TABLE beverage_group MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE certificate_state MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE gift_certificate MODIFY state_id int NOT NULL;
		ALTER TABLE route MODIFY city_id int NOT NULL;
		ALTER TABLE city MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE address MODIFY city_id int NOT NULL;
		ALTER TABLE country MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE city MODIFY country_id int NOT NULL;
		ALTER TABLE coupon MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE tour_order MODIFY coupon_id int NULL;
		ALTER TABLE dinner_table MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE seat MODIFY table_id int NOT NULL;
		ALTER TABLE image MODIFY id int NOT NULL AUTO_INCREMENT;		
		ALTER TABLE menu MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE order_menu MODIFY menu_id int NOT NULL;
		ALTER TABLE tour_menu MODIFY menu_id int NOT NULL;
		ALTER TABLE menu_type MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE menu MODIFY menu_type_id int NOT NULL;
		ALTER TABLE order_seat MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE order_state MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE tour_order MODIFY state_id int NOT NULL;
		ALTER TABLE route MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE route_image MODIFY route_id int NOT NULL;
		ALTER TABLE tour MODIFY route_id int NOT NULL;
		ALTER TABLE route_image MODIFY id int NOT NULL AUTO_INCREMENT;	
		ALTER TABLE seat MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE order_seat MODIFY seat_id int NOT NULL;
		ALTER TABLE surprise MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE order_surprise MODIFY surprise_id int NOT NULL;
		ALTER TABLE gift_certificate MODIFY surprise_id int NULL;
		ALTER TABLE table_category MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE dinner_table MODIFY table_category_id int NOT NULL;
		ALTER TABLE gift_certificate MODIFY table_category_id int NOT NULL;
		ALTER TABLE table_location MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE dinner_table MODIFY table_location_id int NOT NULL;
		ALTER TABLE wine MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE wine_type MODIFY id int NOT NULL AUTO_INCREMENT;
		ALTER TABLE wine MODIFY type_id int NOT NULL;
		ALTER TABLE order_seat MODIFY menu_id int NULL;
	
		ALTER TABLE order_seat ADD CONSTRAINT `order_seat_menu` FOREIGN KEY (`menu_id`) REFERENCES `menu` (`id`);
		ALTER TABLE order_menu ADD CONSTRAINT `order_menu_menu` FOREIGN KEY (`menu_id`) REFERENCES `menu` (`id`);
		ALTER TABLE tour_menu ADD CONSTRAINT `tour_menu_menu` FOREIGN KEY (`menu_id`) REFERENCES `menu` (`id`);	
		ALTER TABLE menu ADD CONSTRAINT `menu_menu_type` FOREIGN KEY (`menu_type_id`) REFERENCES `menu_type` (`id`);
		ALTER TABLE tour_order ADD CONSTRAINT `tour_order_order_state` FOREIGN KEY (`state_id`) REFERENCES `order_state` (`id`);
		ALTER TABLE route_image ADD CONSTRAINT `route_image_route` FOREIGN KEY (`route_id`) REFERENCES `route` (`id`);
		ALTER TABLE tour ADD CONSTRAINT `tour_route` FOREIGN KEY (`route_id`) REFERENCES `route` (`id`);
		ALTER TABLE order_surprise ADD CONSTRAINT `order_surprise_surprise` FOREIGN KEY (`surprise_id`) REFERENCES `surprise` (`id`);
		ALTER TABLE gift_certificate ADD CONSTRAINT `gift_certificate_surprise` FOREIGN KEY (`surprise_id`) REFERENCES `surprise` (`id`);
		ALTER TABLE dinner_table ADD CONSTRAINT `dinner_table_table_category` FOREIGN KEY (`table_category_id`) REFERENCES `table_category` (`id`);
		ALTER TABLE gift_certificate ADD CONSTRAINT `gift_certificate_table_category` FOREIGN KEY (`table_category_id`) REFERENCES `table_category` (`id`);	
		ALTER TABLE dinner_table ADD CONSTRAINT `dinner_table_table_location` FOREIGN KEY (`table_location_id`) REFERENCES `table_location` (`id`);		
		ALTER TABLE order_seat ADD CONSTRAINT `order_seat_seat` FOREIGN KEY (`seat_id`) REFERENCES `seat` (`id`);
		ALTER TABLE city ADD CONSTRAINT `city_country` FOREIGN KEY (`country_id`) REFERENCES `country` (`id`);
		ALTER TABLE address ADD CONSTRAINT `address_city` FOREIGN KEY (`city_id`) REFERENCES `city` (`id`);
		ALTER TABLE gift_certificate ADD CONSTRAINT `gift_certificate_state` FOREIGN KEY (`state_id`) REFERENCES `certificate_state` (`id`);
		ALTER TABLE beverage ADD CONSTRAINT `beverage_beverage_group` FOREIGN KEY (`group_id`) REFERENCES `beverage_group` (`id`);
		ALTER TABLE tour_beverage ADD CONSTRAINT `tour_beverage_beverage` FOREIGN KEY (`beverage_id`) REFERENCES `beverage` (`id`);	
		ALTER TABLE wine ADD CONSTRAINT `wine_beverage` FOREIGN KEY (`id`) REFERENCES `beverage` (`id`);
		ALTER TABLE order_beverage ADD CONSTRAINT `order_beverage_beverage` FOREIGN KEY (`beverage_id`) REFERENCES `beverage` (`id`);
		ALTER TABLE route ADD CONSTRAINT `route_departure_address` FOREIGN KEY (`departure_address_id`) REFERENCES `address` (`id`);
		ALTER TABLE route ADD CONSTRAINT `route_city` FOREIGN KEY (`city_id`) REFERENCES `city` (`id`);		
		ALTER TABLE tour_order ADD CONSTRAINT `tour_order_coupon` FOREIGN KEY (`coupon_id`) REFERENCES `coupon` (`id`);	
		ALTER TABLE seat ADD CONSTRAINT `seat_table` FOREIGN KEY (`table_id`) REFERENCES `dinner_table` (`id`);
		ALTER TABLE wine ADD CONSTRAINT `wine_type` FOREIGN KEY (`type_id`) REFERENCES `wine_type` (`id`);
		ALTER TABLE `route` ADD CONSTRAINT `route_map_image_id` FOREIGN KEY (`map_image_id`) REFERENCES `image` (`id`);
		ALTER TABLE `route` ADD CONSTRAINT `route_title_image_id` FOREIGN KEY (`title_image_id`) REFERENCES `image` (`id`);
		ALTER TABLE `surprise` ADD CONSTRAINT `surprise_image_id` FOREIGN KEY (`image_id`) REFERENCES `image` (`id`);
		ALTER TABLE `beverage` ADD CONSTRAINT `beverage_image_id` FOREIGN KEY (`image_id`) REFERENCES `image` (`id`);
		ALTER TABLE `route_image` ADD CONSTRAINT `route_image_id` FOREIGN KEY (`image_id`) REFERENCES `image` (`id`);
		ALTER TABLE `menu` ADD CONSTRAINT `menu_image_id` FOREIGN KEY (`image_id`) REFERENCES `image` (`id`);		
	END IF;
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE migration;