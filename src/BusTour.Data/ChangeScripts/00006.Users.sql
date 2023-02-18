create table role (
    id varchar(16) not null,
    primary key pk_role (id)
);

create table user (
	id int not null auto_increment,
	user_name varchar(50) not null,
	role varchar(16) not null,
	password text not null,
	password_salt blob not null,
	primary key pk_user (id),
	foreign key fk_user_role (role) references role (id)
);

insert into role (id)
	values ('Supervisor'), ('Administrator'), ('Waiter');