DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if not exists (
		SELECT 1
		FROM information_schema.COLUMNS
		WHERE TABLE_NAME = 'tour' AND COLUMN_NAME = 'number' and TABLE_SCHEMA = DATABASE() 
	)
	then
		alter table tour add column number varchar(128) DEFAULT NULL AFTER id;
  	end if;  
	
	CREATE TABLE IF NOT EXISTS `numbersequence` (
  		`sequence` varchar(128) NOT NULL,
		`number` int NOT NULL DEFAULT(0),
    	PRIMARY KEY (`sequence`)
	) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;	
  	

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;