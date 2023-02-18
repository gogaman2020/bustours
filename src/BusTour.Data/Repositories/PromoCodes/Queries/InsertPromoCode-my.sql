INSERT INTO	promo_code
(
	promo_code_type,
	date_start,
	date_end,
	number_of_promocodes,
	amount_of_discount,
	type_of_discount,
	series_number,
	create_date,
	number_of_uses,
	is_amended,
	city_id
)
VALUES
(
	@PromoCodeType,
	@DateStart,
	@DateEnd,
	@NumberOfPromocodes,
	@AmountOfDiscount,
	@TypeOfDiscount,
	@SeriesNumber,
	@CreateDate,
	0,
	false,
	@CityId
);

select LAST_INSERT_ID();