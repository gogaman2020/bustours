select
	t.id,
	t.departure,
	t.type,
	ts.currentstepname,
	(
	select
		count(os.id)
	from
		tour_order o
	join tourorderstate as tos on
				tos.objectid = o.id
	join order_seat as os on
				os.order_id = o.id
	where 
			o.tour_id = t.id
		and (tos.currentstepname is null || tos.currentstepname in ('TourOrderPaidStep', 'TourOrderDraftStep'))
	) as OccupiedSeatsCount,
	dt.id,
	dt.number,
	dt.is_available as isavailable,
	dt.x,
	dt.y,
	dt.x_size as xsize,
	dt.y_size as ysize,
	dt.is_left as isleft,
	dt.is_right as isright,
	dt.floor,
	dt.price,
	tc.id,
	tc.name,
	s.id,
	s.name,
	s.x,
	s.y,
	s.rotate,
	s.type,
	s.scaleX,
	s.scaleY,
	s.is_forward as isforward,
	s.is_backward as isbackward,
	s.price
from
	tour as t
left outer join dinner_table as dt on
	dt.bus_id = t.bus_id
left outer join table_category as tc on
	tc.id = dt.table_category_id
left outer join seat as s on
	s.table_id = dt.id
left join tourstate ts on
	ts.objectid = t.id
where
	t.route_id = 1
	and t.departure > current_timestamp
	and t.departure < date_add(current_timestamp, interval 6 month)
GROUP BY
	t.id,
	dt.id,
	tc.id,
	s.id,
	ts.id;