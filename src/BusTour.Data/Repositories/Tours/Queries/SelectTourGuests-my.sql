select count(*)
from order_seat as os
left join tour_order as o on o.id =os.order_id 
left join tour as t on t.id=o.tour_id
left join route as r on r.id =t.route_id
where 1 = 1
-- @Ids and o.tour_id in @Ids
