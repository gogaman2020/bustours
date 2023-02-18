UPDATE tour_service_maintenance
SET
	tour_id		= @TourId,
	duration	= @Duration
WHERE id = @Id;