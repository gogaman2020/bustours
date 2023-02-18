insert into tour_menu (tour_id, menu_id, is_ticket, is_extra)
values (@TourId, @MenuId, @IsTicket, @IsExtra)
on duplicate key update id=id;