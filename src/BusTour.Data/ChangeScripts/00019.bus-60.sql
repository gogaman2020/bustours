DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	CREATE TABLE IF NOT EXISTS `payment` (
  		`id` int NOT NULL AUTO_INCREMENT,
  		`order_id` int NULL,
  		`external_id` varchar(256) NULL,
  		`details` json NULL,
    	PRIMARY KEY (`id`),
  		CONSTRAINT `payment_order` FOREIGN KEY (`order_id`) REFERENCES `tour_order` (`id`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
	
	UPDATE tourorderstate 
	SET currentstepname = "TourOrderDraftStep"
	WHERE currentstepname not like '%order%';

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;