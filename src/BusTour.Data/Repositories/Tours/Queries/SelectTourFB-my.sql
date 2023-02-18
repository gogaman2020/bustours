SELECT 
	t.id, 
	t.departure as departure, 
	t.type as tourType, 
	r.name as itinerary,
	t.number as number,
	o.comment as private_hire_comment,
	COALESCE(sm.duration,ph.duration,r.duration) as duration,
	c.name as city,
	ts.currentstepname,
	ph.guest_count as GuestsNumber
FROM tour as t
left join route as r on r.id = t.route_id
left join tour_private_hire ph on ph.tour_id = t.id
left join tour_service_maintenance sm on sm.tour_id = t.id
left join city as c on c.id = r.city_id 
left join tourstate as ts on ts.objectid = t.id
left join tour_order as o on o.tour_id = t.id and t.type = 20
where 1 = 1
-- @Ids and t.id in @Ids
-- @CityId and (r.id is null or r.city_id = @CityId)
-- @Date and DATE(t.departure) = @Date
-- @TourTypes and t.type in @TourTypes
-- @DepartureDateFrom and DATE(t.departure) >= DATE(@DepartureDateFrom)
-- @DepartureDateTo and DATE(t.departure) <= DATE(@DepartureDateTo)
-- @TourProcessStates and ts.currentstepname in @TourProcessStates