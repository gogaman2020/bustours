DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN
	if not exists(SELECT 1
    from INFORMATION_SCHEMA.TABLES
    where table_name = 'common_config' and table_schema = DATABASE()
  ) 
  THEN
    CREATE TABLE common_config (
    id INTEGER NOT NULL AUTO_INCREMENT,
    code VARCHAR(200) COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    value JSON DEFAULT NULL,
    PRIMARY KEY USING BTREE (id)
    ) ENGINE=InnoDB
    AUTO_INCREMENT=1 ROW_FORMAT=DYNAMIC CHARACTER SET 'utf8mb4' COLLATE 'utf8mb4_0900_ai_ci';  
  END IF;  
  
  if not exists (
  select 1 from common_config c
  where c.code = 'promo_code_discount_amount')
  then
    insert into common_config(code,value)
    values('promo_code_discount_amount', '[5,10,15,20]');
  end if;
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;