INSERT INTO	gift_certificate_surprise
(
	certificate_id,
	surprise_id,
	quantity
)
VALUES 
(
	@CertificateId,
	@SurpriseId,
	@Quantity
);

select LAST_INSERT_ID();