SELECT
	p.id,
	p.promo_code_type as PromoCodeType,
	p.date_start as DateStart,
	p.date_end as DateEnd,
	p.number_of_promocodes as NumberOfPromocodes,
	p.number_of_uses as NumberOfUses,
	p.amount_of_discount as AmountOfDiscount,
	p.type_of_discount as TypeOfDiscount,
	p.series_number as SeriesNumber,
	p.create_date as CreateDate,
	p.city_id as CityId,
	p.is_active as IsActive
FROM
	promo_code p
WHERE 1 = 1
-- @Ids and p.id in @Ids
-- @Id and p.id in @Id
-- @SeriesNumber and p.series_number = @SeriesNumber
;