SELECT 
	t.id,
	ot.comment,
	ot.special_requests
FROM tour_order as ot
left join tour as t on t.id = ot.tour_id
left join route as r on r.id =t.route_id
where (ot.comment!='' or ot.special_requests!='')
-- @Ids and t.id in @Ids
-- @CityId and r.city_id = @CityId
-- @Date and DATE(t.departure) = @Date;