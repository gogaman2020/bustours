UPDATE 
	tour_order
SET 
	client_id = @ClientId,
	tour_id = @TourId,
	order_date = @OrderDate,
	payment_date = @PaymentDate,
	table_type = @TableType,
	disabled_guest_count = @DisabledGuestCount,
	promo_code_id = @PromoCodeId,
	certificate_id = @CertificateId,
	discount = @Discount,
	total_price = @TotalPrice,
	is_group = @IsGroup,
	special_requests = @SpecialRequests,
	guest_count = @GuestCount
WHERE 
	id = @Id;