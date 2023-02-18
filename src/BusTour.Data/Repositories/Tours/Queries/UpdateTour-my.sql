UPDATE tour 
SET 
	number = @Number, 
	route_id = @RouteId, 
	bus_id = @BusId, 
	departure = @Departure, 
	seat_price = @SeatPrice, 
	vip_price = @VipPrice, 
	type = @Type 
WHERE id = @Id;