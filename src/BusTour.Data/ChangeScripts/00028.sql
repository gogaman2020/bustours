DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'role' AND COLUMN_NAME = 'id' and TABLE_SCHEMA = DATABASE() 
	)
	then
		ALTER TABLE user DROP FOREIGN KEY `user_ibfk_1`;
		update role set id="Crew" where id="Waiter";
		update user set role="Crew", user_name="crew" where role="Waiter";
		ALTER TABLE user ADD CONSTRAINT `user_ibfk_1` FOREIGN KEY (`role`) REFERENCES `role` (`id`);
  	end if;  

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;



