DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	CREATE TABLE IF NOT EXISTS `notification_template` (
  		`id` int NOT NULL,
		`subject` text NOT NULL,
  		`body` text NOT NULL,
    	PRIMARY KEY (`id`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

	CREATE TABLE IF NOT EXISTS `notification` (
  		`id` int NOT NULL AUTO_INCREMENT,
  		`template_id` int NOT NULL,
  		`email` varchar(256) NOT NULL,
  		`data` json NOT NULL,
		`last_attempt_date` datetime DEFAULT NULL,
		`is_sent` tinyint(1) NOT NULL DEFAULT '0',
    	PRIMARY KEY (`id`),
		CONSTRAINT `notification_notification_template` FOREIGN KEY (`template_id`) REFERENCES `notification_template` (`id`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
	
	if not exists (select 1 from notification_template)
	then
		insert into notification_template (`id`, `subject`, `body`) values
		(10, 'Regular order', '<h1>Regular order</h1><div>Test string: {Id}</div>'),
		(20, 'Group order', '<h1>Group order</h1><div>Test string: {Id}</div>'),
		(30, 'Private hire order', '<h1>Private hire order</h1><div>Test string: {Id}</div>'),
		(40, 'Gift certificate', '<h1>Gift certificate</h1><div>Test string: {Id}</div>'),
		(50, 'Cancel order', '<h1>Cancel order</h1><div>Test string: {Id}</div>');	
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'gift_certificate' AND COLUMN_NAME = 'client_id' and TABLE_SCHEMA = DATABASE() 
	)
	then
		alter table gift_certificate add column client_id int DEFAULT NULL;
		alter table gift_certificate add CONSTRAINT `gift_certificate_client` FOREIGN KEY (`client_id`) REFERENCES `client` (`id`);
  	end if;  
  	

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;