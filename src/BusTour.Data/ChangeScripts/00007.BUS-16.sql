drop procedure if exists migration_script;

DELIMITER $$
create procedure migration_script()
begin
    if not exists(select 1
        from information_schema.tables 
        where table_schema = DATABASE()
        and table_name = "tourorderstate") then
		begin
			create table `tourorderstate`
			(
				 id                 int 			not null 	AUTO_INCREMENT,
				 currentstepname    varchar(200)    not null,
				 statejson          text,
				 constraint pk_tourorderstate__id primary key (`id`)
			);
			
			alter table `tour_order` 
				add `statusid` int null,
				add foreign key fk_tour_order__stateid (statusid) references `tourorderstate`(`id`);
		end;
    END IF;
END$$
DELIMITER ;

call migration_script();
drop procedure migration_script;