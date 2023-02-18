select 
o.*, 
MD5(CONCAT("busberry_",o.id)) as `hash`,
tos.currentstepname,
cl.*,
p.*
-- t.*,
-- ts.currentstepname,
-- DATE_ADD(t.departure, interval TIME_TO_SEC(COALESCE(sm.duration,ph.duration,r.duration)) SECOND) as arrival,
-- ph.*,
-- tos.*
from `tour_order` o
left join `client` cl on cl.id = o.client_id
left join `tourorderstate` tos on tos.objectid = o.id
left join `payment` p on p.order_id = o.id
-- left join `tour` t on t.id = o.tour_id
-- left join `tourstate` ts on ts.objectid = t.id
-- left join `route` r ON r.id = t.route_id
-- left join `tour_private_hire` ph on ph.tour_id = t.id
-- left join `tour_service_maintenance` sm on sm.tour_id = t.id
where 1 = 1
-- @Id and o.id in @Id
-- @TourIds and o.tour_id in @TourIds
-- @OrderProcessStates and tos.currentstepname in @OrderProcessStates
having 1 = 1
-- @Hashes and hash in @Hashes