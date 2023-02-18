update user
set id = @Id,
    user_name = @UserName,
    password = @Password,
    role = @Role,
    password_salt = @PasswordSalt
where id = @Id;