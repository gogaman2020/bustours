CREATE TABLE if not exists `SchemaChangeLog` (
  `ScriptName` text NOT NULL,
  `ExecuteDate` date NOT NULL,
  `ExecuteTime` time NOT NULL,
  KEY `ix_length_mydata` (`ScriptName`(400))
);

create table language (
	code char(2) not null,
    name varchar(64) not null,
    primary key pk_language (code)
);

create table country (
	id smallint not null,
    name json not null,
    primary key pk_country (id)
);

create table city (
	id smallint not null,
    country_id smallint not null,
    name json not null,
    primary key pk_city (id),
    foreign key fk_city_country (country_id) references country (id)
);

create table address (
	id smallint not null,
    city_id smallint not null,
    address_string json not null,
    how_to_get json null,
    primary key pk_address (id),
    foreign key fk_address_city (city_id) references city (id)
);

create table image (
	id int not null,
    file_path varchar(512) not null,
    primary key pk_image (id)
);

create table route (
	id smallint not null,
    name json not null,
    description json null,
    duration time not null,
    city_id smallint not null,
    departure_address_id smallint not null,
    map_image_id int not null,
    title_image_id int not null,
    primary key pk_route (id),
    foreign key fk_route_city (city_id) references city (id),
    foreign key fk_route_address (departure_address_id) references address (id),
    foreign key fk_route_map_image (map_image_id) references image (id),
    foreign key fk_route_title_image (title_image_id) references image (id)
);

create table route_image (
	id smallint not null,
    route_id smallint not null,
    image_id int not null,
    primary key (id),
    foreign key fk_route_image_route (route_id) references route (id),
    foreign key fk_route_image_image (image_id) references image (id)
);

create table bus (
	id smallint not null,
    name varchar(32) null,
    primary key pk_bus (id)
);

create table tour (
	id int not null auto_increment,
    route_id smallint not null,
    bus_id smallint not null,
    departure datetime not null,
    primary key pk_tour (id),
    foreign key fk_tour_route (route_id) references route (id),
    foreign key fk_tour_bus (bus_id) references bus (id)
);

create table order_state (
	id tinyint not null,
    name json not null,
    primary key pk_order_state (id)
);

create table table_category (
	id tinyint not null,
    name json not null,
    primary key pk_table_category (id)
);

create table table_location (
	id tinyint not null,
    name varchar(32) not null,
    primary key pk_table_location (id)
);

create table dinner_table (
	id tinyint not null,
    bus_id smallint not null,
    table_category_id tinyint not null,
    table_location_id tinyint not null,
    number tinyint not null,
    is_available bit not null,
    x smallint not null,
    y smallint not null,
    x_size tinyint not null,
    y_size tinyint not null,
    is_left bit not null,
    is_right bit not null,
    floor tinyint not null,
    price decimal(20,2) not null,
    primary key pk_dinner_table (id),
    foreign key fk_dinner_table_table_category (table_category_id) references table_category (id),
    foreign key fk_dinner_table_table_location (table_location_id) references table_location (id),
    foreign key fk_dinner_table_bus (bus_id) references bus (id)
);

create table seat (
	id smallint not null,
    table_id tinyint not null,
    name char(1) not null,
    x smallint not null,
    y smallint not null,
    is_forward bit not null,
    is_backward bit not null,
    price decimal(20,2) not null,
    primary key pk_seat (id),
    foreign key fk_seat_table (table_id) references dinner_table (id)
);

create table surprise (
	id tinyint not null,
    name json not null,
    price decimal(20,2) not null,
    image_id int null,
    primary key pk_surprise (id),
    foreign key fk_surprise_image (image_id) references image (id)
);

create table certificate_state (
	id tinyint not null,
    name json not null,
    primary key pk_certificate_state (id)
);

create table gift_certificate (
	id int not null auto_increment,
    state_id tinyint not null,
    price tinyint not null,
    end_date datetime not null,
    surprise_id tinyint null,
    table_category_id tinyint not null,
    table_count tinyint null,
    guest_count tinyint not null,
    guest_full_name varchar(256) null,
    guest_phone_number varchar(32) null,
    guest_email varchar(256) not null,
    primary key pk_gift_certificate (id),
    foreign key fk_gift_certificate_state (state_id) references certificate_state (id),
    foreign key fk_gift_certificate_surprise (surprise_id) references surprise (id),
    foreign key fk_gift_certificate_table_category (table_category_id) references table_category (id)
);

create table coupon (
	id smallint not null,
    value varchar(32) not null,
    start_date datetime not null,
    end_date datetime not null,
    is_enabled bit not null,
    primary key pk_coupon (id)
);

create table menu_type (
	id tinyint not null,
    name json not null,
    primary key pk_menu_type (id)
);

create table menu (
	id tinyint not null,
    name json not null,
    price decimal(20,2) not null,
    menu_type_id tinyint not null,
    image_id int null,
    volume decimal(9,3) null,
    unit json null,
    primary key pk_menu (id),
    foreign key fk_menu_type (menu_type_id) references menu_type (id),
    foreign key fk_menu_image (image_id) references image (id)
);

create table beverage_group (
	id tinyint not null,
    name json not null,
    primary key pk_beverage_group (id)
);

create table beverage (
	id smallint not null,
    group_id tinyint not null,
    name json not null,
    volume decimal(9,3) not null,
    unit json not null,
    price decimal(20,2) not null,
    alcohol_by_volume decimal(4,1) null,
    is_hot bit not null,
    image_id int null,
    primary key pk_beverage (id),
    foreign key fk_beverage_group (group_id) references beverage_group (id),
    foreign key fk_beverage_image (image_id) references image (id)
);

create table client (
	id int not null auto_increment,
    full_name varchar(256) null,
    email varchar(256) not null,
    phone_number varchar(32) null,
    is_signed bit not null,
    primary key pk_client (id)
);

create table tour_order (
	id int not null auto_increment,
    state_id tinyint not null,
    client_id int not null,
    tour_id int not null,
    order_date datetime not null,
    payment_date datetime null,
    coupon_id smallint null,
    certificate_id int null,
    discount decimal(20,2) null,
    total_price decimal(20,2) not null,
    primary key pk_tour_order (id),
    foreign key fk_tour_order_state (state_id) references order_state (id),
    foreign key fk_tour_order_tour (tour_id) references tour (id),
    foreign key fk_tour_order_gift_certificate (certificate_id) references gift_certificate (id),
    foreign key fk_tour_order_coupon (coupon_id) references coupon (id),
    foreign key fk_tour_order_client (client_id) references client (id)
);

create table order_seat (
	id int not null auto_increment,
	order_id int not null,
    seat_id smallint not null,
    menu_id tinyint not null,
    primary key pk_order_seat (id),
    foreign key fk_order_seat_order (order_id) references tour_order (id),
    foreign key fk_order_seat_seat (seat_id) references seat (id),
    foreign key fk_order_seat_menu (menu_id) references menu (id)
);

create table order_beverage (
	id int not null auto_increment,
    order_id int not null,
    beverage_id smallint not null,
    amount tinyint not null,
    primary key pk_order_beverage (id),
    foreign key fk_order_beverage_order (order_id) references tour_order (id),
    foreign key fk_order_beverage_beverage (beverage_id) references beverage (id)
);

create table order_menu (
    id int not null auto_increment,
    order_id int not null,
    menu_id tinyint not null,
    amount tinyint not null,
    primary key pk_order_menu (id),
    foreign key fk_order_menu_order (order_id) references tour_order (id),
    foreign key fk_order_menu_menu (menu_id) references menu (id)
);

create table order_surprise (
	id int not null auto_increment,
    order_id int not null,
    surprise_id tinyint not null,
    amount tinyint not null,
    primary key pk_order_surprise (id),
    foreign key fk_order_surprise_order (order_id) references tour_order (id),
    foreign key fk_order_surprise_surprise (surprise_id) references surprise (id)
);

create table wine_type (
	id tinyint not null,
    name json not null,
    primary key pk_wine_type (id)
);

create table wine (
	id smallint not null,
    type_id tinyint not null,
    primary key pk_wine (id),
    foreign key fk_wine_beverage (id) references beverage (id),
    foreign key fk_wine_type (type_id) references wine_type (id)
);