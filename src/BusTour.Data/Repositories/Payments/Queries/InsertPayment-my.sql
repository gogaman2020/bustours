INSERT INTO	payment (
	order_id,
	gift_certificate_id,
	external_id,
	details
) VALUES (
	@OrderId,
	@GiftCertificateId,
	@ExternalId,
	@Details
);

select LAST_INSERT_ID();