select
	s.id as id
from 
	tour as t
	left outer join dinner_table as dt on dt.bus_id = t.bus_id
	left outer join seat as s on s.table_id = dt.id
	left outer join (
		select distinct o.tour_id as tourId, os.seat_id as seatId, true as isLocked
		from
			tour_order o
			left join tourorderstate tos on tos.objectid = o.id
			inner join order_seat os on os.order_id = o.id
		where 
			tos.currentstepname is not null and tos.currentstepname != 'TourOrderCanceledStep'
	) as lockedSeats on lockedSeats.tourId = t.id and lockedSeats.seatId = s.id
where 
	t.id = @TourId
	and lockedSeats.isLocked = true;