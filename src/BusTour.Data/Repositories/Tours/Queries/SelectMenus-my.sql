select
    m.id, m.name, m.price, m.volume, m.unit, i.file_path as imgpath,
    t.id, t.name
from menu as m
    left outer join menu_type as t on t.id = m.menu_type_id
    left outer join image as i on m.image_id = i.id;