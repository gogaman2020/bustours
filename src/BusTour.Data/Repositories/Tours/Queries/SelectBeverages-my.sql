select
    b.id, b.name, b.price, i.file_path as imgpath, b.volume, b.unit, b.alcohol_by_volume as alcoholbyvolume, b.is_hot as ishot,
    g.id, g.name,
    wt.id, wt.name
from beverage as b
    left outer join beverage_group as g on g.id = b.group_id
    left outer join image as i on b.image_id = i.id
    left outer join wine as w on b.id = w.id
    left outer join wine_type as wt on wt.id = w.type_id;