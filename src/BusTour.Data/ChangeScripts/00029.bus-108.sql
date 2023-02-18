﻿DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	insert into notification_template (id, subject, body)
	values (60, 'Booking summary','<h1>Booking summary</h1><div style='width:376px;background-color: #F5F5F5;border-radius:4px'><div><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Number</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{Number}</div></div><div><div style='width:183px;margin-left:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>City</div><div style='width:183px;margin-right:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{City}</div></div><div><div style='width:183px;margin-left:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Date</div><div style='width:183px;margin-right:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{Date}</div></div><div><div style='width:183px;margin-left:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Itinerary</div><div style='width:183px;margin-right:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{Itinerary}</div></div><div><div style='width:183px;margin-left:5px;margin-left:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Departure time</div><div style='width:183px;margin-right:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{DepartureTime}</div></div><div><div style='width:183px;margin-left:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Guests</div><div style='width:183px;margin-right:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{Guests}</div></div><div><div style='width:183px;margin-left:5px;margin-bottom:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Table</div><div style='width:183px;margin-right:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{Table}</div></div></div><div style='width:376px;background-color: #F5F5F5;display:flex;flex-wrap:wrap;border-radius:4px; margin-top:15px'><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;'>Seats</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;'></div><div><div style='width:178px;margin-left:10px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>{Seats}</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{SeatsPrice}</div></div><div><div style='width:183px;margin-left:5px;margin-top:5px;margin-bottom:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Tour price</div><div style='width:183px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;ffont-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>{TourPrice}</div></div></div><div style='width:376px;background-color: #F5F5F5;display:flex;flex-wrap:wrap;border-radius:4px; margin-top:15px'><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;'>Extras</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;'></div><div><div style='width:170px;margin-left:10px;margin-top:5px;margin-bottom:5px;text-align:left;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{ExtrasName}</div><div style='width:31px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{ExtrasCount}</div><div style='width:155px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{ExtrasPrice}</div></div></div><div style='width:376px;background-color: #F5F5F5;display:flex;flex-wrap:wrap;border-radius:4px; margin-top:15px'><div><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Promo code</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{isCode}</div></div><div><div style='width:178px;margin-left:10px;margin-top:5px;margin-bottom:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'></div><div style='width:183px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{CodePrice}</div></div></div><div style='width:376px;background-color: #F5F5F5;display:flex;flex-wrap:wrap;border-radius:4px; margin-top:15px'><div><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Gift certificate</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{isCertificate}</div></div><div><div style='width:178px;margin-left:10px;margin-top:5px;margin-bottom:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'></div><div style='width:183px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{CertificatePrice}</div></div></div><div style='width:376px;background-color: #F5F5F5;display:flex;flex-wrap:wrap;border-radius:4px; margin-top:15px'><div><div style='width:183px;margin-left:5px;margin-top:5px;text-align:left;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>Total</div><div style='width:183px;margin-right:5px;margin-top:5px;text-align:right;font-family: "Muller";font-weight: 700;font-size: 1rem;line-height: 1.5em;display:inline-block'>{TotalPrice}</div></div><div><div style='width:178px;margin-left:10px;margin-top:5px;margin-bottom:5px;text-align:left;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>Including VAT 20%</div><div style='width:183px;margin-right:5px;margin-top:5px;margin-bottom:5px;text-align:right;font-family: "Garamond MT W04", serif;font-weight: 400;display:inline-block'>{VATPrice}</div></div></div>');
	
END$$
DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;