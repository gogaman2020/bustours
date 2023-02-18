DROP PROCEDURE IF EXISTS migration;

DELIMITER //

CREATE PROCEDURE migration()
BEGIN
	IF NOT EXISTS (
		SELECT 1 
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'promo_code' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'city_id'
	) THEN
		ALTER TABLE promo_code
			ADD COLUMN city_id int;
		
		UPDATE promo_code 
		SET city_id = 1;
	END IF;

	IF NOT EXISTS (
		SELECT 1
		FROM information_schema.REFERENTIAL_CONSTRAINTS
		WHERE TABLE_NAME = 'promo_code' AND CONSTRAINT_NAME = 'promo_code__city' AND UNIQUE_CONSTRAINT_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE promo_code
			ADD CONSTRAINT promo_code__city FOREIGN KEY (city_id) REFERENCES city (id);
	END IF;
END//

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;