drop procedure if exists migration_script;

DELIMITER $$
create procedure migration_script()
begin
    if not exists (
		select *
		from information_schema.`COLUMNS`
		where table_schema = DATABASE()
			and table_name = "tourorderstate"
			and COLUMN_NAME = "objectid"
	) then
	begin
		alter table `tourorderstate` 
			add `objectid` int not null default 0;
		
		update `tourorderstate`,
			(
				select o.`id` as obj_id
					 , o.`statusid` as s_id
				from tour_order o
				where o.`statusid` is not null
			) as a
			set `objectid` = a.obj_id
		where a.s_id = `id`;

		alter table tour_order 
			drop constraint `tour_order_ibfk_6`,
			drop column `statusid`;
	end;
	
	end if;

	if not exists (
		select *
		from information_schema.`COLUMNS`
		where table_schema = DATABASE()
			and table_name = "tourstate"
			and COLUMN_NAME = "objectid"
	) then
	begin
		alter table `tourstate` 
			add `objectid` int not null default 0;
		
		update `tourstate`,
			(
				select o.`id` as obj_id
					 , o.`statusid` as s_id
				from tour o
				where o.`statusid` is not null
			) a
			set `objectid` = a.obj_id
		where a.s_id = id;

		alter table tour 
			drop constraint tour_ibfk_3,
			drop column statusid;
	end;
	end if;
end$$
DELIMITER ;

call migration_script();
drop procedure migration_script;