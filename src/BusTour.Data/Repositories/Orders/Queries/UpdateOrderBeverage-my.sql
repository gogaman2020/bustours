UPDATE 
	order_beverage
SET 
	order_id = @OrderId,
	beverage_id = @BeverageId,
	amount = @Amount,
	issued = @Issued
WHERE 
	id = @Id;