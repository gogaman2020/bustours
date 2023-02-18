select `id`, `user_name` as username, `password`, `role`, `password_salt` as passwordsalt
from `user`
where 1=1
-- @Id and id = @id
-- @UserName and user_name = @UserName
;