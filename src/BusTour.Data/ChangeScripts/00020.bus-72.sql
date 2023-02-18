DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
begin
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'payment' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'gift_certificate_id'
	)
	then
		alter table payment add column gift_certificate_id INT NULL AFTER order_id;
	  	alter table payment add CONSTRAINT `payment_gift_certificate` FOREIGN KEY (`gift_certificate_id`) REFERENCES `gift_certificate` (`id`);
  	end if;
  
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'gift_certificate' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'is_paid'
	)
	then
		alter table gift_certificate add column is_paid tinyint(1) NOT NULL DEFAULT '0' AFTER cancelled;
  	end if;  
  
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;