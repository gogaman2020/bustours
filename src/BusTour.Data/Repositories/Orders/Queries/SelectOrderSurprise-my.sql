select s.id, s.order_id, s.amount, s.surprise_id as surpriseId
from `order_surprise` s
where 1=1
-- @OrderId and s.order_id = @OrderId