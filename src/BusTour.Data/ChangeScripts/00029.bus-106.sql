DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
begin

	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'tour_order' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'guest_count'
	)
	then
		alter table tour_order add column guest_count int NOT NULL DEFAULT 0;
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'price'
	)
	then
		alter table order_seat add column price decimal(20,2) NULL;
	end if;	
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;