select T.tour_id, T.menu_id, sum(T.amount) as amount
from
(select o.tour_id, os.menu_id, count(os.menu_id) as amount
from order_seat as os
left join tour_order as o on o.id =os.order_id 
left join tour as t on t.id=o.tour_id
left join route as r on r.id =t.route_id
where 1 = 1
-- @Ids and o.tour_id in @Ids
-- @CityId and r.city_id = @CityId
-- @Date and DATE(t.departure) = @Date
group by os.menu_id, o.tour_id
union all
select o.tour_id, om.menu_id , sum(om.amount) as amount
from order_menu as om
left join tour_order as o on o.id =om.order_id
left join tour as t on t.id=o.tour_id
left join route as r on r.id =t.route_id
left join menu as m on m.id = om.menu_id 
where m.menu_type_id =1
-- @Ids and o.tour_id in @Ids
-- @CityId and r.city_id = @CityId
-- @Date and DATE(t.departure) = @Date
group by om.menu_id, o.tour_id) T
group by T.tour_id, T.menu_id