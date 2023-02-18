INSERT INTO notification (
	object_id,
	template_id,
	email,
	`data`,
	last_attempt_date,
	is_sent
) VALUES (
	@ObjectId,
	@TemplateId,
	@Email,
	@Data,
	@LastAttemptDate,
	@IsSent
);

select LAST_INSERT_ID();