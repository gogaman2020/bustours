INSERT INTO	tour
(
	route_id,
	bus_id,
	departure,
	seat_price,
	vip_price,
	`type`,
	number
)
VALUES(
	@RouteId,
	@BusId,
	@Departure,
	@SeatPrice,
	@VipPrice,
	@Type,
	@Number
);

select LAST_INSERT_ID();