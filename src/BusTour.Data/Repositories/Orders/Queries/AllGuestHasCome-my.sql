UPDATE 
	order_seat
SET 
	guest_has_come = 1
WHERE
	order_id = @OrderId