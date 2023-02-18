insert into tour_beverage (tour_id, beverage_id, is_ticket, is_extra)
values (@TourId, @BeverageId, @IsTicket, @IsExtra)
on duplicate key update id=id;
