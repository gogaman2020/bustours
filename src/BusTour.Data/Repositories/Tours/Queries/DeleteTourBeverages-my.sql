delete from tour_beverage
where tour_id = @TourId and beverage_id not in @BeverageIds;