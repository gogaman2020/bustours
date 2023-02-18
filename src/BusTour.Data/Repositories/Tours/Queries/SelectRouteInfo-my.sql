select
    r.id, r.name, r.description, r.duration as duration,
    ci.name as cityname,
    ci.id as CityId,
    mi.file_path as mapimgpath, ti.file_path as titleimgpath,
    a.address_string as departureaddress, a.how_to_get as departurehowtoget,
    ri.file_path as routeimgpath
from route as r
    left outer join city as ci on r.city_id = ci.id
    left outer join country as co on ci.country_id = co.id
    left outer join image as mi on r.map_image_id = mi.id
    left outer join image as ti on r.title_image_id = ti.id
    left outer join address as a on r.departure_address_id = a.id
    left outer join route_image as r2i on r2i.route_id = r.id
    left outer join image as ri on ri.id = r2i.image_id;