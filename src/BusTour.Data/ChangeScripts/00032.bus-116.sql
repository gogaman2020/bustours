DROP PROCEDURE IF EXISTS migration;

DELIMITER $$
CREATE PROCEDURE migration()
BEGIN

	update address set address_string='{"en": "38-51 Bedford Way, Bloomsbury, London WC1H 0DG", "ru": "Лондон, Бедфорд-уэй, 38-51", "fr": "38-51 Bedford Way, Bloomsbury, Londres WC1H 0DG", "es": "Dirección: 38-51 Bedford Way, Bloomsbury, Londres WC1H 0DG"}', how_to_get='{"en": "You can get to Tower Bridge by several types of public transport: by buses: № 15, 42, 78, 100, 343 (to the stop Tower Bridge or Tower Hill Tower Gateway Station); Metro: Circle and District lines (to Tower Hill station); by river boat: they stop at the St. Katharine Pier and Tower Pier , which are within walking distance to the bridge", "ru": "Доехать до Тауэрского моста можно несколькими видами общественного транспорта: на автобусах: № 15, 42, 78, 100, 343 (до остановки «Tower Bridge» или «Tower Hill Tower Gateway Station»); на метро: линии Circle и District (до станции «Tower Hill»); на речной лодке: они останавливаются у пирсов «St. Katharine Pier» и «Tower Pier», которые находятся в нескольких минутах ходьбы до моста", "fr": "Faites un tour de Londres pour voir des sites anciens et nouveaux, notamment l`abbaye de Westminster, la Tour de Londres et le palais de Buckingham. Admirez des vues étonnantes depuis le London Eye, et il y a même un déjeuner au pub. Chips et petits pois tout le monde?", "es": "Realice un recorrido por Londres para ver lugares de interés antiguos y nuevos, como la Abadía de Westminster, la Torre de Londres y el Palacio de Buckingham. Contempla las asombrosas vistas desde el London Eye, e incluso hay un almuerzo en un pub. Patatas Fritas y guisantes blandos alguien?"}' where id='1';

	update allergy set name='{"en": "Gluten", "ru": "Глютен", "fr": "Gluten", "es": "Gluten"}' where id='1';
	update allergy set name='{"en": "Sugar", "ru": "Сахар", "fr": "Sucre", "es": "Azúcar"}' where id='2';
	update allergy set name='{"en": "Nuts", "ru": "Орехи", "fr": "Noix", "es": "Nueces"}' where id='3';
	update allergy set name='{"en": "Fish", "ru": "Рыба", "fr": "Poisson", "es": "Pez"}' where id='4';
	update allergy set name='{"en": "Other", "ru": "Другое", "fr": "Autre chose", "es": "Lo otro"}' where id='5';

	update beverage set name='{"en": "Still water", "ru": "Негазированная вода", "fr": "Eau plate", "es": "Agua no carbonatada"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='1';
	update beverage set name='{"en": "Sparkling water", "ru": "Газированная вода", "fr": "Eau gazeuse", "es": "Agua gaseosa"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='2';
	update beverage set name='{"en": "Coca-cola", "ru": "Coca-cola", "fr": "Coca-cola", "es": "Coca-cola"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='3';
	update beverage set name='{"en": "Green tea", "ru": "Зелёный чай", "fr": "Thé vert", "es": "Té verde"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='4';
	update beverage set name='{"en": "Black tea", "ru": "Чёрный чай", "fr": "Thé noir", "es": "Té negro"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='5';
	update beverage set name='{"en": "Milk", "ru": "Молоко", "fr": "Lait", "es": "Leche"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='6';
	update beverage set name='{"en": "Beer", "ru": "Пиво", "fr": "Bière", "es": "Cerveza"}', unit='{"en": "ml", "ru": "мл", "fr": "ml", "es": "ml"}' where id='7';
	update beverage set name='{"en": "Cognac", "ru": "Коньяк", "fr": "Cognac", "es": "Coñac"}', unit='{"en": "glass", "ru": "бокал", "fr": "verre", "es": "copa"}' where id='8';
	update beverage set name='{"en": "Cognac", "ru": "Коньяк", "fr": "Cognac", "es": "Coñac"}', unit='{"en": "bottle", "ru": "бутылка", "fr": "bouteille", "es": "botella"}'  where id='9';
	update beverage set name='{"en": "Prosecco", "ru": "Просекко", "fr": "Prosecco", "es": "Prosecco"}', unit='{"en": "glass", "ru": "бокал", "fr": "verre", "es": "copa"}' where id='10';
	update beverage set name='{"en": "Whites wine", "ru": "Белое вино", "fr": "Blanc vin", "es": "Blanco vino"}', unit='{"en": "glass", "ru": "бокал", "fr": "verre", "es": "copa"}' where id='11';
	update beverage set name='{"en": "Whites wine", "ru": "Белое вино", "fr": "Blanc vin", "es": "Blanco vino"}', unit='{"en": "bottle", "ru": "бутылка", "fr": "bouteille", "es": "botella"}' where id='12';
	update beverage set name='{"en": "Reds wine", "ru": "Красное вино", "fr": "Rouge vin", "es": "Rojo vino"}', unit='{"en": "glass", "ru": "бокал", "fr": "verre", "es": "copa"}' where id='13';
	update beverage set name='{"en": "Reds wine", "ru": "Красное вино", "fr": "Rouge vin", "es": "Rojo vino"}', unit='{"en": "bottle", "ru": "бутылка", "fr": "bouteille", "es": "botella"}' where id='14';

	update beverage_group set name='{"en": "Non-alcoholic beverages", "ru": "Безалкогольные напитки", "fr": "Soft-drink", "es": "Refrescos"}' where id='1';
	update beverage_group set name='{"en": "Beer", "ru": "Пиво", "fr": "Bière", "es": "Cerveza"}' where id='2';
	update beverage_group set name='{"en": "Sparkling wine", "ru": "Шампанское", "fr": "Champagne", "es": "Champán"}' where id='3';
	update beverage_group set name='{"en": "Wine", "ru": "Вино", "fr": "Vin", "es": "Vino"}' where id='4';

	update bus set name='{"en": "London BUS", "ru": "Лондонский автобус", "fr": "BUS DE Londres", "es": "AUTOBÚS DE Londres"}' where id='1';

	update certificate_state set name='{"en": "Not used", "ru": "Не использован", "fr": "Non utilisé", "es": "No utilizado"}' where id='1';
	update certificate_state set name='{"en": "Used", "ru": "Использован", "fr": "Exploité", "es": "Usado"}' where id='2';

	update city set name='{"en": "London", "ru": "Лондон", "fr": "Londres", "es": "Londres"}' where id='1';

	update country set name='{"en": "England", "ru": "Англия", "fr": "Angleterre", "es": "Inglaterra"}' where id='1';

	update menu set name='{"en": "Meat", "ru": "Мясное", "fr": "De la viandee", "es": "Carne"}' where id='1';
	update menu set name='{"en": "Fish", "ru": "Рыбное", "fr": "De poisson", "es": "De pescado"}' where id='2';
	update menu set name='{"en": "Vegetarian", "ru": "Овощное", "fr": "De légumes", "es": "De hortalizas"}' where id='3';
	update menu set name='{"en": "Chinese", "ru": "Китайское", "fr": "Chinois", "es": "Chino"}' where id='4';
	update menu set name='{"en": "Black caviar", "ru": "Чёрная икра", "fr": "Caviar", "es": "Caviar negro"}', unit='{"en": "g", "ru": "г", "fr": "g", "es": "g"}' where id='5';
	update menu set name='{"en": "Lobster", "ru": "Лобстер", "fr": "Homard", "es": "Langosta"}', unit='{"en": "item", "ru": "штука", "fr": "pièce", "es": "pieza"}' where id='6';
	update menu set name='{"en": "Oysters", "ru": "Устрицы", "fr": "Huîtres", "es": "Ostras"}', unit='{"en": "item", "ru": "штука", "fr": "pièce", "es": "pieza"}' where id='7';
	update menu set name='{"en": "Éclair", "ru": "Эклер", "fr": "Eclair", "es": "Canuto"}', unit='{"en": "item", "ru": "штука", "fr": "pièce", "es": "pieza"}' where id='8';
	update menu set name='{"en": "Macaron", "ru": "Макарон", "fr": "Macaron", "es": "Macaroon"}', unit='{"en": "item", "ru": "штука", "fr": "pièce", "es": "pieza"}' where id='9';

	update menu_type set name='{"en": "Standard", "ru": "Типовое", "fr": "Typique", "es": "Estándar"}' where id='1';
	update menu_type set name='{"en": "Special", "ru": "Особые случаи", "fr": "Cas à part", "es": "Caso especial"}' where id='2';
	update menu_type set name='{"en": "Desserts", "ru": "Десерты", "fr": "Desserts", "es": "Postres"}' where id='3';

	update route set name='{"en": "Central London sightseeing tour", "ru": "Обзорная экскурсия по центральному Лондону", "fr": "Visite guidée du centre de Londres", "es": "Recorrido por el centro de Londres"}', description='{"en": "Take a London tour to see sights both old and new including Westminster Abbey, the Tower of London and Buckingham Palace. See astonishing views from the London Eye, and there`s even a pub lunch thrown in. Chips and mushy peas anyone?", "ru": "Совершите экскурсию по Лондону, чтобы увидеть старые и новые достопримечательности, включая Вестминстерское аббатство, Лондонский Тауэр и Букингемский дворец. Полюбуйтесь потрясающими видами из Лондонского Глаза и даже обедом в пабе. Чипсы и пюре?", "fr": "Faites un tour de Londres pour voir des sites anciens et nouveaux, notamment l`abbaye de Westminster, la Tour de Londres et le palais de Buckingham. Admirez des vues étonnantes depuis le London Eye, et il y a même un déjeuner au pub. Chips et petits pois tout le monde?", "es": "Realice un recorrido por Londres para ver lugares de interés antiguos y nuevos, como la Abadía de Westminster, la Torre de Londres y el Palacio de Buckingham. Contempla las asombrosas vistas desde el London Eye, e incluso hay un almuerzo en un pub. Patatas Fritas y guisantes blandos alguien?"}' where id='1';

	update surprise set name='{"en": "Sparkling wine", "ru": "Шампанское", "fr": "Champagne", "es": "Champán"}' where id='1';
	update surprise set name='{"en": "Wine", "ru": "Вино", "fr": "Vin", "es": "Vino"}' where id='2';
	update surprise set name='{"en": "Cake", "ru": "Торт", "fr": "Gâteau", "es": "Torta"}' where id='3';
	update surprise set name='{"en": "Cake + candle", "ru": "Торт + свечка", "fr": "Gâteau + chandelle", "es": "Torta + vela"}' where id='4';

	update table_category set name='{"en": "Regular", "ru": "Обычная", "fr": "Vêtement de ville", "es": "Normal"}' where id='1';
	update table_category set name='{"en": "VIP", "ru": "VIP", "fr": "VIP", "es": "VIP"}' where id='2';
	update table_category set name='{"en": "Wheelchair", "ru": "Для инвалидов с коляской", "fr": "Fauteuil", "es": "Silla de Ruedas"}' where id='3';

	update wine_type set name='{"en": "Reds", "ru": "Красное", "fr": "Rouge", "es": "Rojo"}' where id='1';
	update wine_type set name='{"en": "Whites", "ru": "Белое", "fr": "Blanc", "es": "Blanco"}' where id='2';

END$$

DELIMITER ;

CALL migration();

DROP PROCEDURE IF EXISTS migration;





