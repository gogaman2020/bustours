select 
b.*,
t.*,
tc.*,
s.*
from bus b
left join dinner_table t on t.bus_id = b.id
left join table_category tc on tc.id = t.table_category_id
left join seat s on s.table_id = t.id
where 1 = 1
-- @Ids and b.id in @Ids