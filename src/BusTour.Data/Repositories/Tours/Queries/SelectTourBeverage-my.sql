select
m.id,
m.beverage_id as beverageid,
m.tour_id as tourid,
m.is_ticket as isticket,
m.is_extra as isextra
from tour_beverage as m
where 1 = 1
-- @TourId and m.tour_id = @TourId