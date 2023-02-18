UPDATE 
	order_menu
SET 
	order_id = @OrderId,
	menu_id = @MenuId,
	amount = @Amount,
	issued = @Issued
WHERE 
	id = @Id;