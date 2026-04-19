DROP DATABASE IF EXISTS matiedb;
CREATE DATABASE matiedb WITH ENCODING = 'UTF8';

\c matiedb;

CREATE TABLE roles (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE collections (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE service_types (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE booking_statuses (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE qualification_statuses (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE transaction_types (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE review_target_types (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    id           SERIAL PRIMARY KEY,
    username     VARCHAR(100) NOT NULL UNIQUE,
    passwordHash VARCHAR(256) NOT NULL,
    fullName     VARCHAR(200),
    roleId       INTEGER NOT NULL REFERENCES roles(id),
    balance      DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    isActive     BOOLEAN NOT NULL DEFAULT TRUE,
    createdAt    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE services (
    id             SERIAL PRIMARY KEY,
    title          VARCHAR(200) NOT NULL,
    description    TEXT,
    imagePath      VARCHAR(500),
    collectionId   INTEGER NOT NULL REFERENCES collections(id),
    typeId         INTEGER NOT NULL REFERENCES service_types(id),
    price          DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    lastModifiedAt TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE master_services (
    masterId   INTEGER NOT NULL REFERENCES users(id),
    serviceId  INTEGER NOT NULL REFERENCES services(id),
    assignedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (masterId, serviceId)
);

CREATE TABLE bookings (
    id           SERIAL PRIMARY KEY,
    userId       INTEGER NOT NULL REFERENCES users(id),
    serviceId    INTEGER NOT NULL REFERENCES services(id),
    masterId     INTEGER NOT NULL REFERENCES users(id),
    bookingDate  TIMESTAMP NOT NULL,
    statusId     INTEGER NOT NULL REFERENCES booking_statuses(id),
    queueNumber  INTEGER NOT NULL,
    totalPrice   DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    createdAt    TIMESTAMP NOT NULL DEFAULT NOW(),
    lastModifiedAt TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE reviews (
    id             SERIAL PRIMARY KEY,
    userId         INTEGER NOT NULL REFERENCES users(id),
    targetTypeId   INTEGER NOT NULL REFERENCES review_target_types(id),
    targetId       INTEGER NOT NULL,
    rating         INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 5),
    text           TEXT,
    createdAt      TIMESTAMP NOT NULL DEFAULT NOW(),
    lastModifiedAt TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE balance_transactions (
    id           SERIAL PRIMARY KEY,
    userId       INTEGER NOT NULL REFERENCES users(id),
    amount       DECIMAL(12,2) NOT NULL,
    typeId       INTEGER NOT NULL REFERENCES transaction_types(id),
    description  VARCHAR(300),
    balanceAfter DECIMAL(12,2) NOT NULL,
    createdAt    TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE qualification_requests (
    id             SERIAL PRIMARY KEY,
    masterId       INTEGER NOT NULL REFERENCES users(id),
    requestDate    TIMESTAMP NOT NULL DEFAULT NOW(),
    statusId       INTEGER NOT NULL REFERENCES qualification_statuses(id),
    approvedById   INTEGER REFERENCES users(id),
    approvedAt     TIMESTAMP,
    comment        TEXT,
    lastModifiedAt TIMESTAMP NOT NULL DEFAULT NOW()
);

INSERT INTO roles (name) VALUES
    ('Пользователь'),
    ('Администратор'),
    ('Модератор'),
    ('Мастер');

INSERT INTO service_types (name) VALUES
    ('Кастом'),
    ('Косплей');

INSERT INTO collections (name) VALUES
    ('Аниме'),
    ('Новый год'),
    ('Хэллоуин'),
    ('Киберпанк'),
    ('Нуар');

INSERT INTO booking_statuses (name) VALUES
    ('pending'),
    ('confirmed'),
    ('completed'),
    ('cancelled');

INSERT INTO qualification_statuses (name) VALUES
    ('pending'),
    ('approved'),
    ('rejected');

INSERT INTO transaction_types (name) VALUES
    ('topup'),
    ('payment'),
    ('refund');

INSERT INTO review_target_types (name) VALUES
    ('service'),
    ('master');

INSERT INTO users (username, passwordHash, fullName, roleId, balance) VALUES
    ('admin',     '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Администратор системы',          2,      0.00),
    ('moder',     '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Модератор контента',             3,      0.00),
    ('petrov',    '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Петров Александр Иванович',      4,      0.00),
    ('sidorova',  '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Сидорова Мария Николаевна',      4,      0.00),
    ('ivanov',    '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Иванов Дмитрий Сергеевич',       1,  15000.00),
    ('kozlova',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Козлова Анна Павловна',          1,  12000.00),
    ('sokolov',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Соколов Никита Владимирович',    1,  20000.00),
    ('novikova',  '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Новикова Елена Андреевна',       1,   8500.00),
    ('morozov',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Морозов Артём Игоревич',         1,  25000.00),
    ('volkova',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Волкова Дарья Сергеевна',        1,  10000.00),
    ('smirnov',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Смирнов Константин Петрович',    4,      0.00),
    ('frolova',   '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'Фролова Юлия Николаевна',        4,      0.00);

INSERT INTO services (title, description, imagePath, collectionId, typeId, price, lastModifiedAt) VALUES
    ('Плащ в стиле аниме',         'Длинный плащ с капюшоном по мотивам популярных аниме-сериалов',          'Custom/Pr1.jpg',  1, 1,  8000.00, NOW()),
    ('Костюм Наруто Узумаки',      'Полный комплект: куртка, брюки, протектор и повязка Конохи',              'Cosplay/KL1.jpg', 1, 2, 12000.00, NOW()),
    ('Костюм Сейлор Мун',          'Матроска с юбкой, перчатки и аксессуары Сейлор Мун',                     'Cosplay/KL2.jpg', 1, 2, 10000.00, NOW()),
    ('Наряд волшебницы',           'Воздушное платье с накидкой и магическим жезлом',                         'Cosplay/KL3.jpg', 1, 2,  9000.00, NOW()),
    ('Доспехи героя аниме',        'Облегчённые доспехи из EVA-пены с окраской под металл',                   'Cosplay/KL4.jpg', 1, 2, 18000.00, NOW()),

    ('Костюм Деда Мороза',         'Классический красный костюм Деда Мороза с бородой и посохом',             'Custom/Pr2.jpg',  2, 1,  7000.00, NOW()),
    ('Наряд Снегурочки',           'Голубой сарафан с кокошником и шубкой, пошив по меркам',                  'Custom/Pr3.jpg',  2, 1,  6500.00, NOW()),
    ('Новогодняя накидка',         'Бархатная накидка Санты с меховой отделкой',                              'Custom/Pr4.jpg',  2, 1,  4500.00, NOW()),
    ('Рождественский эльф',        'Яркий костюм эльфа с колпаком, поясом и сапожками',                       'Custom/Pr5.jpg',  2, 1,  5500.00, NOW()),

    ('Костюм ведьмы',              'Чёрное платье, шляпа конусом и метла ручной работы',                      'Custom/Pr6.jpg',  3, 1,  5000.00, NOW()),
    ('Плащ вампира',               'Элегантный чёрный плащ с красной подкладкой и клыками',                   'Cosplay/KL5.jpg', 3, 2, 11000.00, NOW()),
    ('Костюм зомби',               'Состаренная одежда с реалистичным аквагримом в комплекте',                 'Cosplay/KL6.jpg', 3, 2,  8500.00, NOW()),
    ('Тыквенный страж',            'Оранжево-чёрный костюм с маской Джека в комплекте',                       'Cosplay/KL7.jpg', 3, 2,  7500.00, NOW()),

    ('Броня самурая-киборга',      'Детальная броня из термопластика с LED-подсветкой',                        'Custom/Pr7.jpg',  4, 1, 20000.00, NOW()),
    ('Светодиодный жилет',         'Жилет с RGB-лентами и управлением с пульта дистанционного управления',    'Custom/Pr8.jpg',  4, 1, 12000.00, NOW()),
    ('Кибер-кроссовки',            'Кастомизированные кроссовки с голографическими деталями',                  'Custom/Pr9.jpg',  4, 1,  6000.00, NOW()),

    ('Детективный плащ',           'Длинный тренчкот 40-х годов с шляпой-федорой',                            'Custom/Pr10.jpg', 5, 1,  9500.00, NOW()),
    ('Винтажный смокинг',          'Смокинг в стиле 30-х: лацканы, бабочка, белая рубашка',                   'Custom/Pr11.jpg', 5, 1, 11000.00, NOW()),
    ('Шляпа гангстера',            'Полный образ гангстера: шляпа, жилет, трость и галстук',                   'Custom/Pr12.jpg', 5, 1,  7000.00, NOW());

INSERT INTO master_services (masterId, serviceId) VALUES
    (3,  1), (3,  6), (3,  7), (3,  8), (3, 14), (3, 15),
    (4,  2), (4,  3), (4,  4), (4, 11), (4, 12),
    (11,  5), (11,  9), (11, 10), (11, 13), (11, 16),
    (12, 17), (12, 18), (12, 19);

INSERT INTO bookings (userId, serviceId, masterId, bookingDate, statusId, queueNumber, totalPrice) VALUES
    (5,   1,  3, '2026-05-10 10:00:00', 3, 1,  8000.00),
    (6,   2,  4, '2026-05-12 11:00:00', 3, 2, 12000.00),
    (7,   3,  4, '2026-05-14 12:00:00', 2, 1, 10000.00),
    (8,   6,  3, '2026-05-15 14:00:00', 2, 3,  7000.00),
    (9,  14,  3, '2026-05-17 09:00:00', 1, 4, 20000.00),
    (10, 17, 12, '2026-05-18 15:00:00', 2, 1,  9500.00),
    (5,  10, 11, '2026-05-20 10:30:00', 3, 2,  5000.00),
    (6,  11,  4, '2026-05-21 13:00:00', 4, 3, 11000.00),
    (7,  15,  3, '2026-05-22 11:00:00', 2, 5, 12000.00),
    (8,   7,  3, '2026-05-23 10:00:00', 3, 6,  6500.00),
    (9,  13, 11, '2026-05-24 14:30:00', 1, 1,  7500.00),
    (10, 18, 12, '2026-05-25 16:00:00', 2, 2, 11000.00);

INSERT INTO reviews (userId, targetTypeId, targetId, rating, text) VALUES
    (5,  1,  1, 5, 'Отличная работа! Плащ превзошёл все ожидания, качество материалов на высоте.'),
    (6,  1,  2, 5, 'Костюм Наруто просто идеален — точно как в аниме. Очень довольна!'),
    (5,  2,  3, 5, 'Петров — настоящий профессионал, всё сделал точно в срок.'),
    (6,  2,  4, 4, 'Сидорова очень внимательна к деталям, но сроки немного затянулись.'),
    (7,  1,  3, 4, 'Костюм Сейлор Мун хороший, но хотелось бы больше блёсток на юбке.'),
    (8,  1,  6, 5, 'Костюм Деда Мороза — восторг! Дети были счастливы, спасибо мастеру.'),
    (8,  2,  3, 5, 'Петров всегда на связи и готов учесть любые пожелания. Рекомендую!'),
    (5,  1, 10, 4, 'Костюм ведьмы смотрится эффектно, грим в подарок — приятный сюрприз.'),
    (9,  1, 14, 5, 'Броня самурая с LED — это нечто! На фестивале все фотографировались со мной.'),
    (10, 1, 17, 4, 'Детективный плащ пошит добротно, шляпа-федора идеально подошла по размеру.'),
    (7,  2, 11, 5, 'Смирнов создал доспехи точно по эскизу. Высшая оценка!'),
    (10, 2, 12, 5, 'Фролова — талантливый мастер, смокинг сидит как влитой.');

INSERT INTO balance_transactions (userId, amount, typeId, description, balanceAfter) VALUES
    (5,  20000.00, 1, 'Пополнение с карты **** 4321',           20000.00),
    (5,  -8000.00, 2, 'Оплата: Плащ в стиле аниме (запись №1)',  12000.00),
    (5,  -5000.00, 2, 'Оплата: Костюм ведьмы (запись №7)',        7000.00),
    (6,  15000.00, 1, 'Пополнение с карты **** 8765',            15000.00),
    (6, -12000.00, 2, 'Оплата: Костюм Наруто (запись №2)',        3000.00),
    (6,   9000.00, 1, 'Пополнение с карты **** 8765',            12000.00),
    (7,  20000.00, 1, 'Пополнение с карты **** 1122',            20000.00),
    (8,  15000.00, 1, 'Пополнение с карты **** 3344',            15000.00),
    (8,  -7000.00, 2, 'Оплата: Наряд Снегурочки (запись №10)',    8000.00),
    (8,    500.00, 3, 'Возврат за отменённую запись №8',          8500.00),
    (9,  25000.00, 1, 'Пополнение с карты **** 5566',            25000.00),
    (10, 10000.00, 1, 'Пополнение с карты **** 7788',            10000.00),
    (10, -9500.00, 2, 'Оплата: Детективный плащ (запись №6)',       500.00),
    (10,  9500.00, 1, 'Пополнение с карты **** 7788',            10000.00),
    (5,   8000.00, 1, 'Пополнение с карты **** 4321',            15000.00);

INSERT INTO qualification_requests (masterId, requestDate, statusId, approvedById, approvedAt, comment) VALUES
    (3,  '2026-01-15 09:00:00', 2, 2, '2026-01-18 10:00:00', 'Мастер подтвердил навыки работы с EVA-пеной. Одобрено.'),
    (4,  '2026-01-20 10:00:00', 2, 1, '2026-01-22 11:00:00', 'Отличное портфолио косплей-костюмов. Одобрено.'),
    (3,  '2026-02-10 08:30:00', 2, 2, '2026-02-12 09:00:00', 'Петров освоил работу с термопластиком и LED. Одобрено.'),
    (11, '2026-02-18 11:00:00', 2, 2, '2026-02-20 14:00:00', 'Смирнов прошёл курс по доспехам. Одобрено.'),
    (12, '2026-02-25 10:00:00', 2, 1, '2026-02-27 12:00:00', 'Фролова — специалист по нуар-стилю. Одобрено.'),
    (4,  '2026-03-05 09:00:00', 3, 2, '2026-03-07 10:00:00', 'Недостаточно работ в портфолио для повышения. Отклонено.'),
    (11, '2026-03-15 14:00:00', 2, 1, '2026-03-17 16:00:00', 'Смирнов успешно завершил дополнительное обучение. Одобрено.'),
    (3,  '2026-04-01 10:00:00', 1, NULL, NULL,               'Заявка на специализацию по кибер-стилю.'),
    (12, '2026-04-05 11:30:00', 1, NULL, NULL,               'Заявка на расширение компетенций (Хэллоуин-направление).'),
    (4,  '2026-04-10 09:00:00', 1, NULL, NULL,               'Повторная заявка после доработки портфолио.');
