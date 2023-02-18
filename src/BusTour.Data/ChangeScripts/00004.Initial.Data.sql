insert into language (code, name)
values ('en', 'English'), ('fr', 'French'), ('ru', 'Russian'), ('zn', 'Chinese');

insert into image (id, file_path)
values (1, 'menu/meat.jpg'), (2, 'menu/fish.jpg'), (3, 'menu/vegetarian.jpg'), (4, 'menu/chinese.jpg'), (5, 'menu/black-caviar.jpg'), (6, 'menu/lobster.jpg'), (7, 'menu/oysters.jpg'),
	(8, 'menu/eclair.jpg'), (9, 'menu/macaron.jpg'),
    (10, 'route/1/title.jpg'), (11, 'route/1/map.jpg'),
    (12, 'route/1/1.jpg'), (13, 'route/1/2.jpg'), (14, 'route/1/3.jpg'), (15, 'route/1/4.jpg'), (16, 'route/1/5.jpg'), (17, 'route/1/6.jpg'), (18, 'route/1/7.jpg');

insert into country (id, name)
values (1, json_object('en', 'England', 'ru', 'Англия'));

insert into city (id, country_id, name)
values (1, 1, json_object('en', 'London', 'ru', 'Лондон'));

insert into address (id, city_id, address_string, how_to_get)
values (1, 1, json_object('en', '38-51 Bedford Way, Bloomsbury, London WC1H 0DG', 'ru', 'Лондон, Бедфорд-уэй, 38-51'),
	json_object(
		'en',
		'You can get to Tower Bridge by several types of public transport: by buses: № 15, 42, 78, 100, 343 (to the stop "Tower Bridge" or "Tower Hill Tower Gateway Station"); Metro: Circle and District lines (to Tower Hill station); by river boat: they stop at the St. Katharine Pier "and" Tower Pier ", which are within walking distance to the bridge',
        'ru',
        'Доехать до Тауэрского моста можно несколькими видами общественного транспорта: на автобусах: № 15, 42, 78, 100, 343 (до остановки «Tower Bridge» или «Tower Hill Tower Gateway Station»); на метро: линии Circle и District (до станции «Tower Hill»); на речной лодке: они останавливаются у пирсов «St. Katharine Pier» и «Tower Pier», которые находятся в нескольких минутах ходьбы до моста'
	)
);

insert into route (id, name, duration, city_id, departure_address_id, title_image_id, map_image_id, description)
values (1, json_object('en', 'Central London sightseeing tour', 'ru', 'Обзорная экскурсия по центральному Лондону'), '02:00:00', 1, 1, 10, 11,
	json_object(
		'en',
        'Take a London tour to see sights both old and new including Westminster Abbey, the Tower of London and Buckingham Palace. See astonishing views from the London Eye, and there''s even a pub lunch thrown in. Chips and mushy peas anyone?',
        'ru',
        'Совершите экскурсию по Лондону, чтобы увидеть старые и новые достопримечательности, включая Вестминстерское аббатство, Лондонский Тауэр и Букингемский дворец. Полюбуйтесь потрясающими видами из Лондонского Глаза и даже обедом в пабе. Чипсы и пюре?'
	)
);

insert into route_image (id, route_id, image_id)
values (1, 1, 12), (2, 1, 13), (3, 1, 14), (4, 1, 15), (5, 1, 16), (6, 1, 17), (7, 1, 18);

insert into bus (id, name)
values (1, 'DIVO London Bus');

/*create tour schedule*/
drop temporary table if exists schedule;
create temporary table schedule (
	departure time not null
);

insert into schedule (departure)
values ('09:00:00'), ('12:00:00'), ('15:00:00'), ('18:00:00');

select date(now()) into @`today` from dual;

set @@cte_max_recursion_depth = 5000;
insert into tour (route_id, bus_id, departure)
with recursive cte as
(
   select 1 as i
   union all
   select i + 1
   from cte
   where i < 3000
)
select 1, 1, timestamp(date_add(@`today`, interval i day), s.departure) as departure_time
from schedule as s, cte
where dayofweek(date_add(@`today`, interval i day)) in (3,6,7,1)
order by departure_time;

drop temporary table if exists schedule;
/*end create tour schedule*/

insert into order_state (id, name)
values (1, json_object('en', 'Draft', 'ru', 'Черновик')), (2, json_object('en', 'Completed', 'ru', 'Сформирован')),
	(3, json_object('en', 'Paid', 'ru', 'Оплачен')), (4, json_object('en', 'Canceled', 'ru', 'Отменён'));

insert into table_category (id, name)
values (1, json_object('en', 'Regular', 'ru', 'Обычная')), (2, json_object('en', 'VIP', 'ru', 'VIP')), (3, json_object('en', 'Wheelchair', 'ru', 'Для инвалидов с коляской'));

insert into table_location (id, name)
values (1, 'Regular'), (2, 'FirstRow'), (3, 'LastRow');

insert into dinner_table (id, bus_id, table_category_id, table_location_id, number, is_available, x, y, x_size, y_size, is_left, is_right, floor, price)
values
	(1, 1, 2, 2, 1, 1, 860, 89, 1, 2, 0, 1, 2, 120), (2, 1, 2, 2, 2, 1, 860, 6, 1, 2, 1, 0, 2, 120), (3, 1, 1, 1, 1, 1, 728, 122, 2, 1, 0, 1, 2, 80),
	(4, 1, 1, 1, 2, 1, 728, 6, 2, 2, 1, 0, 2, 160), (5, 1, 1, 1, 3, 1, 597, 122, 2, 1, 0, 1, 2, 80), (6, 1, 1, 1, 4, 1, 597, 6, 2, 2, 1, 0, 2, 160),
	(7, 1, 1, 1, 5, 1, 465, 122, 2, 1, 0, 1, 2, 80), (8, 1, 1, 1, 6, 1, 465, 6, 2, 2, 1, 0, 2, 160), (9, 1, 1, 1, 7, 1, 330, 122, 2, 1, 0, 1, 2, 80),
	(10, 1, 1, 1, 8, 1, 68, 5, 2, 2, 1, 0, 2, 160), (11, 1, 1, 1, 9, 1, 199, 122, 2, 1, 0, 1, 2, 80), (12, 1, 1, 1, 11, 1, 68, 89, 2, 2, 0, 1, 2, 160),
	(13, 1, 1, 1, 12, 1, 584, 122, 2, 1, 0, 1, 1, 80), (14, 1, 1, 1, 14, 1, 644, 5, 1, 2, 1, 0, 1, 80), (15, 1, 1, 1, 15, 1, 510, 5, 2, 2, 1, 0, 1, 160),
	(16, 1, 3, 1, 16, 1, 434, 101, 1, 1, 0, 1, 1, 60), (17, 1, 1, 1, 17, 1, 339, 89, 2, 2, 0, 1, 1, 80), (18, 1, 1, 1, 18, 1, 237, 89, 2, 2, 0, 1, 1, 80),
	(19, 1, 1, 3, 19, 1, 139, 89, 2, 2, 0, 1, 1, 80), (20, 1, 1, 3, 20, 1, 139, 5, 2, 2, 1, 0, 1, 80);

insert into seat (id, table_id, name, x, y, is_forward, is_backward, price)
values
	(1, 1, 'A', 821, 126, 1, 0, 70), (2, 1, 'B', 821, 89, 1, 0, 70),
	(3, 2, 'C', 821, 43, 1, 0, 70), (4, 2, 'D', 821, 6, 1, 0, 70),
    (5, 3, 'A', 689, 126, 1, 0, 50), (6, 3, 'B', 778, 126, 0, 1, 50),
    (7, 4, 'A', 689, 6, 1, 0, 50), (8, 4, 'B', 778, 6, 0, 1, 50), (9, 4, 'C', 689, 43, 1, 0, 50), (10, 4, 'D', 778, 43, 0, 1, 50),
    (11, 5, 'A', 558, 126, 1, 0, 50), (12, 5, 'B', 648, 126, 0, 1, 50),
    (13, 6, 'A', 557, 6, 1, 0, 50), (14, 6, 'B', 648, 6, 0, 1, 50), (15, 6, 'C', 557, 43, 1, 0, 50), (16, 6, 'D', 648, 43, 0, 1, 50),
    (17, 7, 'A', 426, 126, 1, 0, 50), (18, 7, 'B', 517, 126, 0, 1, 50),
    (19, 8, 'A', 426, 6, 1, 0, 50), (20, 8, 'B', 515, 6, 0, 1, 50), (21, 8, 'C', 426, 43, 1, 0, 50), (22, 8, 'D', 515, 43, 0, 1, 50),
    (23, 9, 'A', 291, 126, 1, 0, 50), (24, 9, 'B', 381, 126, 0, 1, 50),
    (25, 10, 'A', 30, 5, 1, 0, 50), (26, 10, 'B', 119, 6, 0, 1, 50), (27, 10, 'C', 30, 41, 1, 0, 50), (28, 10, 'D', 119, 42, 0, 1, 50),
    (29, 11, 'A', 160, 126, 1, 0, 50), (30, 11, 'B', 249, 126, 0, 1, 50),
    (31, 12, 'A', 30, 126, 1, 0, 50), (32, 12, 'B', 119, 126, 0, 1, 50), (33, 12, 'C', 30, 90, 1, 0, 50), (34, 12, 'D', 119, 89, 0, 1, 50),
    (35, 13, 'A', 544, 126, 1, 0, 50), (36, 13, 'B', 635, 126, 0, 1, 50),
    (37, 14, 'A', 605, 5, 1, 0, 50), (38, 14, 'C', 605, 42, 1, 0, 50),
    (39, 15, 'A', 469, 5, 1, 0, 50), (40, 15, 'B', 562, 5, 0, 1, 50), (41, 15, 'C', 469, 42, 1, 0, 50), (42, 15, 'D', 562, 42, 0, 1, 50),
    (43, 16, 'A', 396, 118, 1, 0, 60),
    (44, 17, 'A', 298, 126, 1, 0, 50), (45, 17, 'C', 298, 89, 1, 0, 50),
    (46, 18, 'A', 196, 126, 1, 0, 50), (47, 18, 'C', 196, 89, 1, 0, 50),
    (48, 19, 'A', 99, 126, 1, 0, 50), (49, 19, 'C', 99, 89, 1, 0, 50),
    (50, 20, 'A', 99, 5, 1, 0, 50), (51, 20, 'C', 99, 42, 1, 0, 50);

insert into surprise (id, name, price, image_id)
values (1, json_object('en', 'Sparkling wine', 'ru', 'Шампанское'), 21.99, null), (2, json_object('en', 'Wine', 'ru', 'Вино'), 21.99, null),
	(3, json_object('en', 'Cake', 'ru', 'Торт'), 21.99, null), (4, json_object('en', 'Cake + candle', 'ru', 'Торт + свечка'), 21.99, null);

insert into certificate_state (id, name)
value (1, json_object('en', 'Not used', 'ru', 'Не использован')), (2, json_object('en', 'Used', 'ru', 'Использован'));

insert into menu_type (id, name)
values (1, json_object('en', 'Standard', 'ru', 'Типовое')), (2, json_object('en', 'Special', 'ru', 'Особые случаи')), (3, json_object('en', 'Desserts', 'ru', 'Десерты'));

insert into menu (id, name, price, menu_type_id, image_id, volume, unit)
values (1, json_object('en', 'Meat', 'ru', 'Мясное'), 23.99, 1, 1, null, null), (2, json_object('en', 'Fish', 'ru', 'Рыбное'), 18.99, 1, 2, null, null),
	(3, json_object('en', 'Vegetarian', 'ru', 'Овощное'), 17.99, 1, 3, null, null), (4, json_object('en', 'Chinese', 'ru', 'Китайское'), 21.99, 1, 4, null, null),
    (5, json_object('en', 'Black caviar', 'ru', 'Чёрная икра'), 49.99, 2, 5, 200, json_object('en', 'g', 'ru', 'г')),
    (6, json_object('en', 'Lobster', 'ru', 'Лобстер'), 18.99, 2, 6, 1, json_object('en', 'item', 'ru', 'штука')),
	(7, json_object('en', 'Oysters', 'ru', 'Устрицы'), 17.99, 2, 7, 6, json_object('en', 'items', 'ru', 'штук')),
    (8, json_object('en', 'Éclair', 'ru', 'Эклер'), 3.99, 3, 8, null, null), (9, json_object('en', 'Macaron', 'ru', 'Макарон'), 2.99, 3, 9, null, null);

insert into beverage_group (id, name)
values (1, json_object('en', 'Non-alcoholic beverages', 'ru', 'Безалкогольные напитки')), (2, json_object('en', 'Beer', 'ru', 'Пиво')),
	(3, json_object('en', 'Sparkling wine', 'ru', 'Шампанское')), (4, json_object('en', 'Wine', 'ru', 'Вино'));

insert into wine_type (id, name)
values (1, json_object('en', 'Reds', 'ru', 'Красное')), (2, json_object('en', 'Whites', 'ru', 'Белое'));

insert into beverage (id, group_id, name, volume, unit, price, alcohol_by_volume, is_hot, image_id)
values
	(1, 1, json_object('en', 'Still water', 'ru', 'Негазированная вода'), 180, json_object('en', 'ml', 'ru', 'мл'), 0.49, null, 0, null),
    (2, 1, json_object('en', 'Sparkling water', 'ru', 'Газированная вода'), 180, json_object('en', 'ml', 'ru', 'мл'), 0.49, null, 0, null),
    (3, 1, json_object('en', 'Coca-cola', 'ru', 'Coca-cola'), 250, json_object('en', 'ml', 'ru', 'мл'), 1.99, null, 0, null),
    (4, 1, json_object('en', 'Green tea', 'ru', 'Зелёный чай'), 120, json_object('en', 'ml', 'ru', 'мл'), 1.49, null, 1, null),
    (5, 1, json_object('en', 'Black tea', 'ru', 'Чёрный чай'), 120, json_object('en', 'ml', 'ru', 'мл'), 1.49, null, 1, null),
    (6, 1, json_object('en', 'Milk', 'ru', 'Молоко'), 200, json_object('en', 'ml', 'ru', 'мл'), 2.29, null, 0, null),
    (7, 2, json_object('en', 'Kronenbourg 1664 (France)', 'ru', 'Kronenbourg 1664 (Франция)'), 330, json_object('en', 'ml', 'ru', 'мл'), 2.29, 5.5, 0, null),
    (8, 3, json_object('en', 'Henri Giraud Hommage á François Hémart Grand Cru N.V.', 'ru', 'Henri Giraud Hommage á François Hémart Grand Cru N.V.'), 1, json_object('en', 'glass', 'ru', 'бокал'), 45.00, 11.5, 0, null),
    (9, 3, json_object('en', 'Henri Giraud Hommage á François Hémart Grand Cru N.V.', 'ru', 'Henri Giraud Hommage á François Hémart Grand Cru N.V.'), 1, json_object('en', 'bottle', 'ru', 'бутылка'), 160.00, 11.5, 0, null),
    (10, 3, json_object('en', 'Prosecco Extra Dry Villa Domiziano N.V.', 'ru', 'Prosecco Extra Dry Villa Domiziano N.V.'), 1, json_object('en', 'glass', 'ru', 'бокал'), 45.00, 10.5, 0, null),
    (11, 4, json_object('en', 'Riesling, Karl Schaefer, Germany 2017', 'ru', 'Riesling, Karl Schaefer, Германия 2017'), 1, json_object('en', 'glass', 'ru', 'бокал'), 13.00, 12.5, 0, null),
    (12, 4, json_object('en', 'Riesling, Karl Schaefer, Germany 2017', 'ru', 'Riesling, Karl Schaefer, Германия 2017'), 1, json_object('en', 'bottle', 'ru', 'бутылка'), 36.00, 12.5, 0, null),
    (13, 4, json_object('en', 'Serre Nuove, Tenuta dell’ Ornellaia Italy 2017', 'ru', 'Serre Nuove, Tenuta dell’ Ornellaia Италия 2017'), 1, json_object('en', 'glass', 'ru', 'бокал'), 27.0, 13.0, 0, null),
    (14, 4, json_object('en', 'Serre Nuove, Tenuta dell’ Ornellaia Italy 2017', 'ru', 'Serre Nuove, Tenuta dell’ Ornellaia Италия 2017'), 1, json_object('en', 'bottle', 'ru', 'бутылка'), 78.00, 13.0, 0, null);

insert into wine (id, type_id)
values (8, 2), (9, 2), (10, 2), (11, 2), (12, 2), (13, 1), (14, 1);