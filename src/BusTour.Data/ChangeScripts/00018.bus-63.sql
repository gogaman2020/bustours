DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
begin
	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'gift_certificate' and TABLE_SCHEMA = DATABASE() AND COLUMN_NAME = 'number'
	)
	then
		alter table gift_certificate add column number VARCHAR(10) NOT NULL AFTER id;
	end if;
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;