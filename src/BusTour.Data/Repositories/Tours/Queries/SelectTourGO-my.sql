select count(ot.id)
FROM tour as t
left join tour_order as ot on ot.tour_id = t.id and ot.is_group=1
where 1 = 1
-- @Ids and t.id in @Ids