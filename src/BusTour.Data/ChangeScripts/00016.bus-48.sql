DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	if exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour' and column_name = 'type' and table_schema = DATABASE()) 
	THEN
		UPDATE tour
		SET TYPE = 0
		WHERE TYPE IS NULL;
		
		ALTER TABLE tour MODIFY COLUMN type tinyint NOT NULL DEFAULT 0;
	end if;  
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_order' and column_name = 'type' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour_order ADD COLUMN type tinyint NOT NULL DEFAULT 0;
	end if;  
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_private_hire' and column_name = 'price' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour_private_hire ADD COLUMN price decimal(20,2) DEFAULT NULL;
	end if;  	
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_order' and column_name = 'comment' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour_order ADD COLUMN comment varchar(1024) DEFAULT NULL;
	end if;  	
	
	if not exists(select 1
	from INFORMATION_SCHEMA.columns
	where table_name = 'tour_order' and column_name = 'special_requests' and table_schema = DATABASE()) 
	then
		ALTER TABLE tour_order ADD COLUMN special_requests varchar(1024) DEFAULT NULL;
	end if; 	
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;