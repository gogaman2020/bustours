DROP PROCEDURE IF EXISTS migration;

DELIMITER //

CREATE PROCEDURE migration()
BEGIN
	-- Ranaming coupon_id to promo_code_id, dropping useless table 'coupon'
	IF EXISTS (
		SELECT 1
		FROM information_schema.REFERENTIAL_CONSTRAINTS
		WHERE 
			TABLE_NAME = 'tour_order' AND 
			CONSTRAINT_NAME = 'tour_order_coupon' AND
			UNIQUE_CONSTRAINT_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE tour_order
			DROP CONSTRAINT tour_order_coupon;
	END IF;
	
	IF EXISTS (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE 
			TABLE_NAME = 'tour_order' AND 
			COLUMN_NAME = 'coupon_id' AND 
			TABLE_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE tour_order 
			RENAME COLUMN coupon_id TO promo_code_id;
	END IF;
	
	IF EXISTS (
		SELECT 1
		FROM information_schema.`COLUMNS`
		WHERE 
			TABLE_NAME = 'tour_order' AND 
			COLUMN_NAME = 'promo_code_id' AND 
			TABLE_SCHEMA = DATABASE()
	) THEN
		IF NOT EXISTS (
			SELECT 1 
			FROM information_schema.REFERENTIAL_CONSTRAINTS
			WHERE 
				CONSTRAINT_NAME = 'tour_order__promo_code' AND
				UNIQUE_CONSTRAINT_SCHEMA = DATABASE()
		) THEN
			ALTER TABLE tour_order
				ADD CONSTRAINT tour_order__promo_code FOREIGN KEY (promo_code_id) REFERENCES promo_code(id);
		END IF;
	END IF;

	IF EXISTS (
		SELECT 1
		FROM information_schema.TABLES
		WHERE
			TABLE_NAME = 'coupon' AND 
			TABLE_SCHEMA = DATABASE()
	) THEN
		DROP TABLE coupon;
	END IF;

	-- Adding missing column to order: count of guest with disabilities, selected table type
	IF NOT EXISTS (
		SELECT 1
		FROM information_schema.`COLUMNS`
		WHERE 
			TABLE_NAME = 'tour_order' AND
			COLUMN_NAME = 'table_type' AND 
			TABLE_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE tour_order
			ADD COLUMN table_type int NOT NULL DEFAULT 1 AFTER order_date;
	END IF;

	IF NOT EXISTS (
		SELECT 1
		FROM information_schema.`COLUMNS`
		WHERE
			TABLE_NAME = 'tour_order' AND
			COLUMN_NAME = 'disabled_guest_count' AND
			TABLE_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE tour_order
			ADD COLUMN disabled_guest_count int NOT NULL DEFAULT 0 AFTER table_type;
	END IF;
END//

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;