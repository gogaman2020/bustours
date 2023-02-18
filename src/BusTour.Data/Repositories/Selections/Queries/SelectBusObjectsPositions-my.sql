select
	dt.id, dt.x, dt.y,
	s.id, s.x, s.y
from 
	tour as t
	left outer join dinner_table as dt on dt.bus_id = t.bus_id
	left outer join seat as s on s.table_id = dt.id
where t.id = @TourId;