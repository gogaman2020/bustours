SELECT
	t.id,
	t.route_id,
	t.bus_id,
	t.departure,
	t.seat_price as seatprice,
	t.vip_price as vipprice,
	t.`type`,
	DATE_ADD(t.departure, interval TIME_TO_SEC(COALESCE(sm.duration,ph.duration,r.duration)) SECOND) as arrival,
	(select count(distinct o.id) 
		from tour_order o
		join tourorderstate os on os.objectid = o.id
		WHERE o.tour_id = t.id and os.currentstepname not in ('TourOrderNotPaidStep','TourOrderCanceledStep')
	) as ordersCount,
	ts.currentstepname,
	t.number
FROM tour t
LEFT JOIN route r ON r.id = t.route_id
left join tour_private_hire ph on ph.tour_id = t.id
left join tour_service_maintenance sm on sm.tour_id = t.id
left join tourstate ts on ts.objectid = t.id
where 1 = 1
-- @Ids and t.id in @Ids
-- @Id and t.id in @Id
-- @TourTypes and t.type in @TourTypes
-- @DepartureDateFrom and t.departure >= @DepartureDateFrom
-- @DepartureDateTo and t.departure <= @DepartureDateTo
-- @BlockBookingDateFromEnd and (ph.id is null or ph.block_booking_date_from <= @BlockBookingDateFromEnd)
-- @BlockBookingDateToStart and (ph.id is null or ph.block_booking_date_to >= @BlockBookingDateToStart)
-- @CityId and r.city_id = @CityId
-- @RouteId and r.id = @RouteId
-- @BusId and t.bus_id = @BusId
-- @TourProcessStates and ts.currentstepname in @TourProcessStates
having 1 = 1
-- @ArrivalDateFrom and arrival >= @ArrivalDateFrom
-- @ArrivalDateTo and arrival <= @ArrivalDateTo
-- @HasOrders and (t.`type` != 0 or (((@HasOrders = true and ordersCount > 0) or (@HasOrders = false and ordersCount = 0 ))))
-- @Limit LIMIT @Limit
-- @Offset OFFSET @Offset
;