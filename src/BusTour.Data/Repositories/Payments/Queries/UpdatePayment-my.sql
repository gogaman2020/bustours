UPDATE
	payment
SET
	order_id = @OrderId,
	gift_certificate_id = @GiftCertificateId,
	external_id = @ExternalId,
	details = @Details
WHERE
	id = @Id;