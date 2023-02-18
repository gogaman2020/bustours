insert into user (id, user_name, password, role, password_salt)
    values (@Id, @UserName, @Password, @Role, @PasswordSalt);
SELECT LAST_INSERT_ID();