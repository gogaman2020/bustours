UPDATE 
	order_seat
SET 
	order_id = @OrderId,
	seat_id = @SeatId,
	guest_has_come = @GuestHasCome,
	menu_id = @MenuId,
	has_menu_issued = @HasMenuIssued,
	is_empty = @IsEmpty,
	beverage_id = @BeverageId,
	has_beverage_issued = @HasBeverageIssued,
	allergy_id = @AllergyId,
	other_allergy = @OtherAllergy,
	price = @Price
WHERE 
	id = @Id;