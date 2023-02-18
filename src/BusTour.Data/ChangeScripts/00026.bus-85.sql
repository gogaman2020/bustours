DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
begin
	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_beverage' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'issued'
	)
	then
		alter table order_beverage change issued issued tinyint(1) not null DEFAULT 0;
	end if;
	
	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_menu' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'issued'
	)
	then
		alter table order_menu change issued issued tinyint(1) not null DEFAULT 0;
	end if;
	
	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'guest_has_come'
	)
	then
		alter table order_seat change guest_has_come guest_has_come tinyint(1) not null DEFAULT 0;
	end if;
	
	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'has_menu_issued'
	)
	then
		alter table order_seat change has_menu_issued has_menu_issued tinyint(1) not null DEFAULT 0;
	end if;
	
	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'order_seat' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'has_beverage_issued'
	)
	then
		alter table order_seat change has_beverage_issued has_beverage_issued tinyint(1) not null DEFAULT 0;
	end if;
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;