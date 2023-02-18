DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	IF NOT EXISTS(SELECT 1
		FROM INFORMATION_SCHEMA.TABLES
		WHERE table_name = 'gift_certificate_amount_variant_' and table_schema = DATABASE()
	) 
	THEN
	
		CREATE TABLE IF NOT EXISTS `gift_certificate_amount_variant` (
			`id` int NOT NULL AUTO_INCREMENT,
			`amount` decimal(20,2) NOT NULL,
			PRIMARY KEY (`id`)
		) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
		
		IF NOT EXISTS(SELECT 1 from gift_certificate_amount_variant) 
		THEN
			insert into gift_certificate_amount_variant (amount) values (150),(250),(350),(350),(500),(1000);
		END IF;	
		
		ALTER TABLE tour_order DROP FOREIGN KEY `tour_order_ibfk_3`;
		-- ALTER TABLE tour_order DROP FOREIGN KEY `tour_order_gift_certificate`;

		DROP TABLE `gift_certificate`;
		
		CREATE TABLE `gift_certificate` (
		  `id` int NOT NULL AUTO_INCREMENT,
		  `date_start` datetime NOT NULL,
		  `date_end` datetime NOT NULL,
		  `amount_variant_id` int NULL,
		  `amount` decimal(20,2) NULL,
		  `comment` varchar(512) NULL,
		  PRIMARY KEY (`id`),
		  KEY `fk_gift_certificate_amount_variant` (`amount_variant_id`),
		  CONSTRAINT `gift_certificate_ibfk_1` FOREIGN KEY (`amount_variant_id`) REFERENCES `gift_certificate_amount_variant` (`id`)
		) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;	
		
		ALTER TABLE tour_order ADD CONSTRAINT `tour_order_gift_certificate` FOREIGN KEY (`certificate_id`) REFERENCES `gift_certificate` (`id`);	
		
		CREATE TABLE IF NOT EXISTS `gift_certificate_surprise` (
		  `id` int NOT NULL AUTO_INCREMENT,
		  `certificate_id` int NOT NULL,
		  `surprise_id` int NOT NULL,
		  PRIMARY KEY (`id`),
		  CONSTRAINT `gift_certificate_surprise_certificate` FOREIGN KEY (`certificate_id`) REFERENCES `gift_certificate` (`id`),
		  CONSTRAINT `gift_certificate_surprise_surprise` FOREIGN KEY (`surprise_id`) REFERENCES `surprise` (`id`)
		) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
		
		ALTER TABLE gift_certificate_surprise ADD quantity int NOT NULL;		
		
	END IF;	

END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;