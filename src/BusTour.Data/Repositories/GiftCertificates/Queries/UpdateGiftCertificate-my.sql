UPDATE
	gift_certificate
SET
	number = @Number,
	date_start = @DateStart,
	date_end = @DateEnd,
	amount_variant_id = @AmountVariantId,
	redeemed_amount = @RedeemedAmount,
	redeemed_date = @RedeemedDate,
	comment = @Comment,
	cancelled = @Cancelled,
	is_paid = @IsPaid,
	client_id = @ClientId
WHERE
	id = @Id;