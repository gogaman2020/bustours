drop procedure if exists migration_script;

DELIMITER $$
create procedure migration_script()
begin
    if not exists(select 1
        from information_schema.tables 
        where table_schema = DATABASE()
        and table_name = "tourstate") then
		begin
			create table `tourstate`
			(
				 id                 int 			not null 	AUTO_INCREMENT,
				 currentstepname    varchar(200)    not null,
				 statejson          text,
				 constraint pk_tourstate__id primary key (`id`)
			);
			
			alter table `tour` 
				add `statusid` int null,
				add foreign key fk_tour__stateid (statusid) references `tourstate`(`id`);
		end;
    END IF;
END$$
DELIMITER ;

call migration_script();
drop procedure migration_script;