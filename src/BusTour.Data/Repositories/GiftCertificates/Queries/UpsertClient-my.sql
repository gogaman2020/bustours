insert into client (full_name, email, phone_number, is_signed)
select @FullName, @Email, @PhoneNumber, @IsSigned
where not exists (select 1 from client where COALESCE(email,'') = COALESCE(@Email,'') and COALESCE(phone_number,'') = COALESCE(@PhoneNumber,''));

select id from client where COALESCE(email,'') = COALESCE(@Email,'') and COALESCE(phone_number,'') = COALESCE(@PhoneNumber,'') limit 1;