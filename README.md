# money-service

## Скрипты для заполнения базы шаблонными данными

```
CREATE TABLE public."Users"
(
	"UserId" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 ),
	"Username" text,
	"Password" text,
	"Salt" text
);

ALTER TABLE public."Users"
    OWNER to postgres;


CREATE TABLE public."Account"
(
    "AccountBalance" integer,
    "AccountOwnersId" integer,
    "AccountNumber" bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 40000000000 MINVALUE 40000000000 MAXVALUE 49000000000 CACHE 1 )
);

ALTER TABLE public."Account"
    OWNER to postgres;


-- Пароль у всех пользозвателей: "pas"
 INSERT INTO public."Users" ("UserId", "Username", "Password", "Salt")
VALUES 
(1, 'user1@mail.ru', '1B8A828E354C209FB04C74253967F8B1', 'd6d5073c'),
(2, 'user2@mail.ru', '11B74F9B8B3D2B403B4230657EB6E144', '4da940cd'),
(3, 'user3@mail.ru', '4D28BC09D464B3ECD01E5EF51C42E0F1', '051329c7'),
(4, 'user4@mail.ru', '9C9F0F9407483901C48EB7095B31790F', '4b4d5162'),
(5, 'user5@mail.ru', '2D11EDED7092D09EC821AA380B69FC39', '8f826a85');


INSERT INTO public."Account" ("AccountBalance", "AccountOwnersId", "AccountNumber")
OVERRIDING SYSTEM VALUE 
VALUES 
(9000, 1, 40000000001),
(15000, 2, 40000000002),
(7000, 3, 40000000003),
(500, 4, 40000000004),
(46000, 5, 40000000005),
(0, 1, 40000000006);

```
