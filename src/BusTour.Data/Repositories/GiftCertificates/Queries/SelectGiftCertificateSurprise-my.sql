SELECT
	t.id,
	t.certificate_id as CertificateId,
	t.surprise_id as SurpriseId,
	t.quantity
FROM
	gift_certificate_surprise t
WHERE 1 = 1
-- @CertificateId AND t.certificate_id = @CertificateId 
;