insert into	tour_order (
	client_id,
	tour_id,
	order_date,
	payment_date,
	table_type,
	disabled_guest_count,
	promo_code_id,
	certificate_id,
	discount,
	total_price,
	comment,
	special_requests,
	is_group,
	guest_count
) select
	@ClientId,
	@TourId,
	@OrderDate,
	@PaymentDate,
	@TableType,
	@DisabledGuestCount,
	@PromoCodeId,
	@CertificateId,
	@Discount,
	@TotalPrice,
	@Comment,
	@SpecialRequests,
	@IsGroup,
	@GuestCount;

select
	LAST_INSERT_ID();