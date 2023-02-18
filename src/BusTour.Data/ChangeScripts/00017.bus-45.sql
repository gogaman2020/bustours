DROP PROCEDURE IF EXISTS migration;

DELIMITER //

CREATE PROCEDURE migration()
BEGIN
	-- @Deletion old state references
	IF EXISTS (
		SELECT 1 
		FROM information_schema.REFERENTIAL_CONSTRAINTS 
		WHERE CONSTRAINT_NAME = 'tour_order_order_state' AND TABLE_NAME = 'tour_order' and UNIQUE_CONSTRAINT_SCHEMA = DATABASE()
	) THEN 
		ALTER TABLE tour_order
			DROP CONSTRAINT tour_order_order_state;
	END IF;
	
	IF EXISTS (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'tour_order' AND COLUMN_NAME = 'state_id' and table_schema = DATABASE()
	) THEN 
		ALTER TABLE tour_order 
			DROP COLUMN state_id;
	END IF;

	IF EXISTS (
		SELECT 1
		FROM information_schema.TABLES
		WHERE TABLE_NAME = 'order_state' and table_schema = DATABASE()
	) THEN 
		DROP TABLE order_state;
	END IF;

	-- Fill state tables for existings tours and orders
	-- Filling tours states when state is not defined
	IF EXISTS (
		SELECT 1
		FROM information_schema.TABLES
		WHERE TABLE_NAME = 'tour' and table_schema = DATABASE()
	) THEN 
		IF EXISTS (
			SELECT 1
			FROM information_schema.TABLES
			WHERE TABLE_NAME = 'tourstate' and table_schema = DATABASE()
		) THEN 
			INSERT INTO tourstate (currentstepname, statejson, objectid)
			SELECT 'TourDraftStep', '{}', t.id
			FROM tour t
			LEFT JOIN tourstate ts ON ts.objectid = t.id
			WHERE ts.currentstepname IS NULL;
		END IF;
	END IF;

	-- Filling orders states when state is not defined
	IF EXISTS (
		SELECT 1
		FROM information_schema.TABLES
		WHERE TABLE_NAME = 'tour_order' and table_schema = DATABASE()
	) THEN 
		IF EXISTS (
			SELECT 1
			FROM information_schema.TABLES
			WHERE TABLE_NAME = 'tourorderstate' and table_schema = DATABASE()
		) THEN 
			INSERT INTO tourorderstate (currentstepname, statejson, objectid)
			SELECT 'TourOrderDraftStep', '{}', o.id
			FROM tour_order o
			LEFT JOIN tourorderstate tos ON tos.objectid = o.id
			WHERE tos.currentstepname IS NULL;
		END IF;
	END IF;
END//

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;