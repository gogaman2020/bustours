select 
n.*,
nt.*
from notification n
join notification_template nt on nt.id = n.template_id
where 1 = 1
-- @IsSent and n.is_sent = @IsSent
-- @HasEmail and n.email is not null and n.email != ''
-- @Id and n.id = @Id
;