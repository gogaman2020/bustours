SELECT
/*Section_Body
	p.id,
	p.number_of_promocodes as NumberOfPromocodes,
	p.series_number as SeriesNumber,
	p.type_of_discount as DiscountType,
	p.amount_of_discount as AmountOfDiscount,
	p.create_date as CreationDate,
	p.date_start as DateStart,
	p.date_end as ExpirationDate,
	p.number_of_uses as QuantityUsed,
	p.promo_code_type as PromoCodeType,
	p.is_active as IsActive
--EndSection*/

/*Section_Count
	count(p.id)
--EndSection*/

/*Section_From
	FROM promo_code p
	JOIN city c on c.id = p.city_id
	WHERE 1 = 1
--EndSection*/

/*Section_ByFilter
-- @City and LCASE(c.name) LIKE CONCAT('%', LCASE(@City), '%')
-- @ApplyStatusFilter and
-- @IsExpiredFilter not
-- @ApplyStatusFilter ((p.date_end is null or p.date_start is null or (p.date_start < @UtcDateTimeUtcNow and @UtcDateTimeUtcNow < p.date_end)) and (p.number_of_promocodes is null or p.number_of_uses is null or p.number_of_promocodes > p.number_of_uses))
and is_active = 1
--EndSection*/
;