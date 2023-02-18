select
    t.id, t.departure,
    dt.id, dt.number, dt.is_available as isavailable, dt.x, dt.y, dt.x_size as xsize, dt.y_size as ysize, dt.is_left as isleft, dt.is_right as isright, dt.floor, dt.price,
    tc.id, tc.name,
    s.id, s.name, s.x, s.y, s.is_forward as isforward, s.is_backward as isbackward, s.price,
    ph.*,
    sm.*
from tour as t
    left outer join dinner_table as dt on dt.bus_id = t.bus_id
    left outer join table_category as tc on tc.id = dt.table_category_id
    left outer join seat as s on s.table_id = dt.id
    left join tour_private_hire ph on ph.tour_id = t.id
    left join tour_service_maintenance sm on sm.tour_id = t.id
where 1=1
-- @Id and t.id = @Id