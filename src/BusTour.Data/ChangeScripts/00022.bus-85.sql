DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
begin
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_beverage' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'issued'
	)
	then
		alter table order_beverage add column issued tinyint(1) NOT NULL AFTER amount;
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_menu' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'issued'
	)
	then
		alter table order_menu add column issued tinyint(1) NOT NULL AFTER amount;
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'guest_has_come'
	)
	then
		alter table order_seat add column guest_has_come tinyint(1) NOT NULL AFTER seat_id;
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'has_menu_issued'
	)
	then
		alter table order_seat add column has_menu_issued tinyint(1) NOT NULL AFTER menu_id;
	end if;
	
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'has_beverage_issued'
	)
	then
		alter table order_seat add column has_beverage_issued tinyint(1) NOT NULL AFTER beverage_id;
	end if;
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;