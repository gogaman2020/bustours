delete from tour_menu
where tour_id = @TourId and menu_id not in @MenuIds;