INSERT INTO	gift_certificate
(
	number,
	date_start,
	date_end,
	amount_variant_id,
	redeemed_amount,
	redeemed_date,
	`comment`,
	cancelled,
	is_paid,
	client_id
)
VALUES
(
	@Number,
	@DateStart,
	@DateEnd,
	@AmountVariantId,
	@RedeemedAmount,
	@RedeemedDate,
	@Comment,
	@Cancelled,
	@IsPaid,
	@ClientId
);

select LAST_INSERT_ID();