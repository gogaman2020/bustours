UPDATE
	notification
SET
	template_id = @TemplateId,
	email = @Email,
	`data` = @Data,
	last_attempt_date = @LastAttemptDate,
	is_sent = @IsSent,
	object_id = @ObjectId
WHERE
	id = @Id;