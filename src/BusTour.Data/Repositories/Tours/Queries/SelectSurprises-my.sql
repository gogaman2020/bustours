select s.id, s.name, s.price, i.file_path as imgpath
from surprise as s
    left outer join image as i on s.image_id = i.id;