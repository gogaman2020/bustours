DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	update seat set y = 9 where y = 8;

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;





