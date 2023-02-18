DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

		update notification_template set 
		body = "Dear {Customer},<br><br>We really appreciate your booking, and we have been trying our best to ensure your great experience with us.<br>Unfortunately, due to some technical issues that are outside of our control we must cancel your booking number <b>{BookingNumber}</b> for <b>{BookingDate}</b>.<br>Could you please provide us your card details (or bank account details) to make the refund.<br>We sincerely apologise for any inconvenience caused.<br><br>We hope wo see you next time.<br><br>With regards Customer Support"
		where subject = 'Cancel order';
	
END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;