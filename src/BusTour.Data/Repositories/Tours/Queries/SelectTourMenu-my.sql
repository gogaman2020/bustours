select
m.id,
m.menu_id as menuid,
m.tour_id as tourid,
m.is_ticket as isticket,
m.is_extra as isextra
from tour_menu as m
where 1 = 1
-- @TourId and m.tour_id = @TourId