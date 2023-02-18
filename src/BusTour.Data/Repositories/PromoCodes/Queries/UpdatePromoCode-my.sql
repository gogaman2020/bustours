UPDATE promo_code
SET
	promo_code_type			= @PromoCodeType,
	date_start				= @DateStart,
	date_end				= @DateEnd,
	number_of_promocodes	= @NumberOfPromocodes,
	amount_of_discount		= @AmountOfDiscount,
	type_of_discount		= @TypeOfDiscount,
	series_number			= @SeriesNumber,
	is_amended				= 1,
	is_active				= @IsActive,
	number_of_uses          = @NumberOfUses
WHERE id = @Id