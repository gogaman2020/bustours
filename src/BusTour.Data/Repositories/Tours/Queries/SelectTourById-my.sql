SELECT 
	t.id, 
	t.departure as departure, 
	t.type as tourType, 
	r.name as itinerary,
	r.duration as duration,
	c.name as city,
	ts.currentstepname as status
FROM tour as t
left join route as r on r.id =t.route_id
left join city as c on c.id = r.city_id 
left join tourstate as ts on ts.objectid = t.id
where 1 = 1
-- @Ids and t.id in @Ids
