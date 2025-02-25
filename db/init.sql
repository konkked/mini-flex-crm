-- Start of file: init/_db/01_create_database.sql -------------------------


CREATE DATABASE miniflexcrm;
\c miniflexcrm;

-- End of file: init/_db/01_create_database.sql-----------------------------

-- Start of file: init/company/01_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NOT NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);


-- End of file: init/company/01_create_table.sql-----------------------------

-- Start of file: init/customer/01_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "customer" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NOT NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);


-- End of file: init/customer/01_create_table.sql-----------------------------

-- Start of file: init/relation/01_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "relation" (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customer(id),
    entity VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL
);


-- End of file: init/relation/01_create_table.sql-----------------------------

-- Start of file: init/relation/02_create_index.sql -------------------------


CREATE INDEX IF NOT EXISTS idx_relation_customer_entity_id
ON relation (customer_id, entity, entity_id);

-- End of file: init/relation/02_create_index.sql-----------------------------

-- Start of file: init/relation/03_create_get_customer_entities.sql -------------------------


CREATE OR REPLACE FUNCTION get_customer_entities(customer_id INT) RETURNS JSON AS $$
DECLARE
    result JSON := '{}'::JSON; -- Initialize result as an empty JSON object
    entity_name TEXT;
    entity_data JSON;
BEGIN
    -- Loop through each entity related to the customer
    FOR entity_name IN
        SELECT DISTINCT entity FROM relation WHERE customer_id = customer_id
    LOOP
        -- Execute dynamic SQL to fetch data for the current entity
        EXECUTE format('SELECT json_agg(row_to_json(t)) FROM (SELECT * FROM %I WHERE id IN (SELECT entity_id FROM relation WHERE customer_id = $1 AND entity = $2)) t', entity_name)
        INTO entity_data
        USING customer_id, entity_name;

        -- If entity_data is not null, add it to the result JSON object
        IF entity_data IS NOT NULL THEN
            result := result || json_build_object(entity_name, entity_data);
        END IF;
    END LOOP;

    RETURN result;
END;
$$ LANGUAGE plpgsql;

-- End of file: init/relation/03_create_get_customer_entities.sql-----------------------------

-- Start of file: init/tenant/01_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "tenant" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL
);

-- Insert tenant record with id 0 if it does not exist
INSERT INTO "tenant" (id, name)
VALUES (0, 'root')
ON CONFLICT (id) DO NOTHING;

-- Set the sequence to start from 1 if the current value is less than 1
DO $$
BEGIN
    IF (SELECT setval(pg_get_serial_sequence('tenant', 'id'), 1, false)) < 1 THEN
        PERFORM setval(pg_get_serial_sequence('tenant', 'id'), 1, false);
    END IF;
END $$;


-- End of file: init/tenant/01_create_table.sql-----------------------------

-- Start of file: init/user/01_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "user" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    role VARCHAR(50) NOT NULL,
    enabled boolean default false,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);


-- End of file: init/user/01_create_table.sql-----------------------------

-- Start of file: init/xtn/01_add_metadata_columns.sql -------------------------


DO $$
DECLARE
    table_name TEXT;
BEGIN
    FOR table_name IN
        SELECT tablename
        FROM pg_tables
        WHERE schemaname = 'public'  -- Adjust schema if necessary
    LOOP
        EXECUTE format('
            ALTER TABLE %I
            ADD COLUMN IF NOT EXISTS created_ts BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
            ADD COLUMN IF NOT EXISTS updated_ts BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
            ADD COLUMN IF NOT EXISTS updated_by_id INT NOT NULL DEFAULT 0;
        ', table_name);
    END LOOP;
END $$;

-- End of file: init/xtn/01_add_metadata_columns.sql-----------------------------

-- Start of file: init/xtn/02_update_ts_fn.sql -------------------------


CREATE OR REPLACE FUNCTION update_updated_ts()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_ts = EXTRACT(EPOCH FROM NOW());
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- End of file: init/xtn/02_update_ts_fn.sql-----------------------------

-- Start of file: init/xtn/03_add_update_ts_triggers.sql -------------------------


DO $$
DECLARE
    table_name TEXT;
    trigger_name TEXT;
BEGIN
    FOR table_name IN
        SELECT tablename
        FROM pg_tables
        WHERE schemaname = 'public'  -- Adjust schema if necessary
    LOOP
        -- Generate a unique trigger name for the table
        trigger_name := 'trg_update_updated_ts_' || table_name;

        -- Check if the trigger already exists
        IF NOT EXISTS (
            SELECT 1
            FROM pg_trigger
            WHERE tgname = trigger_name
              AND tgrelid = (SELECT oid FROM pg_class WHERE relname = table_name AND relnamespace = 'public'::regnamespace)
        ) THEN
            -- Create the trigger if it doesn't exist
            EXECUTE format('
                CREATE TRIGGER %I
                BEFORE UPDATE ON %I
                FOR EACH ROW
                EXECUTE FUNCTION update_updated_ts();
            ', trigger_name, table_name);
        END IF;
    END LOOP;
END $$;

-- End of file: init/xtn/03_add_update_ts_triggers.sql-----------------------------

