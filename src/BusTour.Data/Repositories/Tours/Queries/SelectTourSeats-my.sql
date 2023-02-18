SELECT 
	count(s.id)
FROM tour as t
left join dinner_table as dt on dt.bus_id = t.bus_id 
left join seat as s on s.table_id = dt.id 
where 1 = 1
-- @Ids and t.id in @Ids