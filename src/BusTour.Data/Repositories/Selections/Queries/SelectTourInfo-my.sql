select
	dt.id as id, dt.number as number, dt.floor as floor, dt.table_category_id as tableCategory, tl.id as tableLocation,
	s.id as id, s.name as name, lockedSeats.isLocked as isLocked, lockedSeats.orderId as orderId, s.type as type
from 
	tour as t
	left outer join dinner_table as dt on dt.bus_id = t.bus_id
	left outer join table_location as tl on tl.id = dt.table_location_id
	left outer join seat as s on s.table_id = dt.id
	left outer join (
		select distinct o.tour_id as tourId, os.seat_id as seatId, true as isLocked, o.id as orderId
		from
			tour_order o
			inner join order_seat os on os.order_id = o.id
			left join tourorderstate tos on tos.objectid = o.id
		where 
			tos.currentstepname is not null and tos.currentstepname != 'TourOrderCanceledStep'
	) as lockedSeats on lockedSeats.tourId = t.id and lockedSeats.seatId = s.id
where 
	t.id = @TourId 
	and dt.is_available = true;