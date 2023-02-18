INSERT INTO order_seat (
	order_id,
	seat_id,
	menu_id,
	is_empty,
	beverage_id,
	allergy_id,
	other_allergy,
	price
)
VALUES (
	@OrderId, 
	@SeatId, 
	@MenuId, 
	@IsEmpty,
	@BeverageId,
	@AllergyId,
	@OtherAllergy,
	@Price
);