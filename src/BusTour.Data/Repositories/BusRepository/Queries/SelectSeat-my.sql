select 
s.*,
t.*,
tc.*
from seat s
join dinner_table t on t.id = s.table_id
join table_category tc on tc.id = t.table_category_id
where 1 = 1
-- @Ids and s.id in @Ids
-- @BusIds and t.bus_id in @BusIds;