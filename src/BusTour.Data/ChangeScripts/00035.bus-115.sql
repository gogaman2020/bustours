DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	UPDATE seat SET y = y + 2 where table_id = 17 and name in ('A','B','C');
	UPDATE seat SET y = y - 2 where table_id = 17 and name in ('D','E','F');
	UPDATE seat SET y = y + 2 where table_id = 17 and name in ('C');
	UPDATE seat SET y = y - 2 where table_id = 17 and name in ('D');

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;





