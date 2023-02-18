UPDATE
	tour_private_hire
SET
	tour_id = @TourId,
	duration = @Duration,
	block_booking_date_from = @BlockBookingDateFrom,
	block_booking_date_to = @BlockBookingDateTo,
	block_booking_for_draft = @BlockBookingForDraft,
	departure_point = @DeparturePoint,
	arrival_point = @ArrivalPoint,
	stops = @Stops,
	price = @Price,
	guest_count = @GuestCount
WHERE
	id = @Id;
