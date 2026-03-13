-- Таблицы для модуля цехов (выполнить при необходимости)

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'workshops' AND xtype = 'U')
BEGIN
    CREATE TABLE workshops (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(200) NOT NULL
    );
    INSERT INTO workshops (name) VALUES (N'Сборочный цех'), (N'Раскройный цех'), (N'Малярный цех');
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'product_workshop' AND xtype = 'U')
BEGIN
    CREATE TABLE product_workshop (
        product_id INT NOT NULL,
        workshop_id INT NOT NULL,
        workers_count INT NOT NULL DEFAULT 1,
        production_time_hours DECIMAL(10,2) NOT NULL DEFAULT 0,
        PRIMARY KEY (product_id, workshop_id),
        FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
        FOREIGN KEY (workshop_id) REFERENCES workshops(id) ON DELETE CASCADE
    );
    -- Пример: привязать первые 2 цеха к первому продукту
    INSERT INTO product_workshop (product_id, workshop_id, workers_count, production_time_hours)
    SELECT 1, id, 3, 2.5 FROM workshops WHERE id <= 2;
END
