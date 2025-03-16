-- Start of file: ./00_db/01_create_database.sql -------------------------

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'miniflexcrm') THEN
        CREATE DATABASE miniflexcrm;
    END IF;
END $$;

\c miniflexcrm


-- End of file: ./00_db/01_create_database.sql ---------------------------

-- Start of file: ./01_tenant/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "tenant" (
    id SERIAL PRIMARY KEY,
    short_id VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) UNIQUE NOT NULL,
    theme VARCHAR(255) NOT NULL,
    attributes JSON NULL
);
-- End of file: ./01_tenant/01_create_table.sql ---------------------------

-- Start of file: ./01_tenant/02_create_root_tenant.sql -------------------------


-- Insert tenant record with id 0 if it does not exist
INSERT INTO "tenant" (id, name, short_id, theme) VALUES (0, 'root', 'root', 'professional') ON CONFLICT (id) DO NOTHING;

-- End of file: ./01_tenant/02_create_root_tenant.sql ---------------------------

-- Start of file: ./01_tenant/03_fix_tenant_table_id.sql -------------------------

-- Set the sequence to start from the maximum id value in the tenant table plus one
DO $$ 
DECLARE
    max_id INTEGER;
BEGIN
    SELECT MAX(id) INTO max_id FROM tenant;
    IF max_id IS NULL THEN
        max_id := 1;
    ELSE
        max_id := max_id + 1;
    END IF;
    PERFORM setval(pg_get_serial_sequence('tenant', 'id'), max_id, false);
END $$;
-- End of file: ./01_tenant/03_fix_tenant_table_id.sql ---------------------------

-- Start of file: ./01_tenant/04_add_logo_and_banner_to_tenant.sql -------------------------

ALTER TABLE tenant
ADD COLUMN logo BYTEA DEFAULT NULL,
ADD COLUMN banner BYTEA DEFAULT NULL;
-- End of file: ./01_tenant/04_add_logo_and_banner_to_tenant.sql ---------------------------

-- Start of file: ./02_user/01_create_user_role.sql -------------------------

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_role') THEN
        CREATE TYPE user_role AS ENUM ('admin', 'standard', 'guest');
    END IF;
END $$;

-- End of file: ./02_user/01_create_user_role.sql ---------------------------

-- Start of file: ./02_user/02_create_table.sql -------------------------


CREATE TABLE IF NOT EXISTS "app_user" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(512) UNIQUE NOT NULL,
    attributes JSON NULL,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    role user_role NOT NULL DEFAULT 'standard',
    enabled boolean default false,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);
-- End of file: ./02_user/02_create_table.sql ---------------------------

-- Start of file: ./02_user/03_create_super_admin.sql -------------------------


DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM "app_user" WHERE username = 'super_admin') THEN
            INSERT INTO "app_user" (id, username, name, email, password_hash, salt, role, enabled, tenant_id)
            VALUES (0, 'super_admin', 'Darth Keyser', '1chkeyser1@gmail.com','o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 
                        'g8eY0NC2wVYWA2y4chGP7Q==','admin', true, 0);
            RAISE NOTICE 'Super admin created';
        END IF;
END $$;

-- End of file: ./02_user/03_create_super_admin.sql ---------------------------

-- Start of file: ./02_user/04_fix_user_table_id.sql -------------------------

DO $$
DECLARE
    new_id integer = 0;
BEGIN
    SELECT MAX(id) + 1 INTO new_id FROM "app_user";
    PERFORM setval(pg_get_serial_sequence('app_user', 'id'), GREATEST(1, new_id));
END $$;
-- End of file: ./02_user/04_fix_user_table_id.sql ---------------------------

-- Start of file: ./03.1_contact copy/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);

-- End of file: ./03.1_contact copy/01_create_table.sql ---------------------------

-- Start of file: ./03.1_contact/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);

-- End of file: ./03.1_contact/01_create_table.sql ---------------------------

-- Start of file: ./03.2_location/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);

-- End of file: ./03.2_location/01_create_table.sql ---------------------------

-- Start of file: ./03_company/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);

-- End of file: ./03_company/01_create_table.sql ---------------------------

-- Start of file: ./04_customer/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "customer" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(512) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id),
    CONSTRAINT unique_company_name_per_tenant UNIQUE (tenant_id, name)
);


-- End of file: ./04_customer/01_create_table.sql ---------------------------

-- Start of file: ./05_relationship/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "relationships" (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customer(id),
    entity VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL
);

-- End of file: ./05_relationship/01_create_table.sql ---------------------------

-- Start of file: ./05_relationship/02_create_index.sql -------------------------

CREATE INDEX IF NOT EXISTS idx_relationships_customer_entity_id
ON relationships (customer_id, entity, entity_id);
-- End of file: ./05_relationship/02_create_index.sql ---------------------------

-- Start of file: ./05_relationship/03_create_get_customer_relationships.sql -------------------------

CREATE OR REPLACE FUNCTION get_customer_relationships(customer_id INT) RETURNS JSON AS $$
DECLARE
    result JSONB := '{}'::JSONB; -- Initialize result as an empty JSONB object
    entity_name TEXT;
    entity_data JSONB; -- Use JSONB for entity_data as well
BEGIN
    -- Loop through each entity related to the customer
    FOR entity_name IN
        SELECT DISTINCT entity 
        FROM relationships 
        WHERE relationships.customer_id = get_customer_relationships.customer_id
    LOOP
        -- Execute dynamic SQL to fetch data for the current entity, ensure empty array if no data
        EXECUTE format('SELECT COALESCE(jsonb_agg(row_to_json(t)), ''[]''::JSONB) FROM (SELECT * FROM %I WHERE id IN (SELECT entity_id FROM relationships WHERE relationships.customer_id = $1 AND entity = $2)) t', entity_name)
        INTO entity_data
        USING get_customer_relationships.customer_id, entity_name;

        -- Add the entity data to the result JSONB object
        result := result || jsonb_build_object(entity_name, entity_data);
    END LOOP;

    -- Convert JSONB to JSON for the return type
    RETURN result::JSON;
END;
$$ LANGUAGE plpgsql;
-- End of file: ./05_relationship/03_create_get_customer_relationships.sql ---------------------------

-- Start of file: ./06_note/01_create_table.sql -------------------------

CREATE TABLE IF NOT EXISTS "note" (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES app_user(id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    route VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    content VARCHAR(512) NOT NULL,
    pinned BOOLEAN DEFAULT FALSE
);
-- End of file: ./06_note/01_create_table.sql ---------------------------

-- Start of file: ./06_note/02_create_index.sql -------------------------

CREATE INDEX IF NOT EXISTS idx_note_tenantId_userId_route
ON note (tenant_id, user_id, route);
-- End of file: ./06_note/02_create_index.sql ---------------------------

-- Start of file: ./999_seed/01_seed_tenants.sql -------------------------

INSERT INTO "tenant" (short_id, name, theme, attributes) VALUES
('abc123', 'Tenant One', 'professional', '{}'),
('def456', 'Tenant Two', 'enterprise', '{}'),
('ghi789', 'Tenant Three', 'social', '{}')
ON CONFLICT DO NOTHING;

SELECT * FROM "tenant";
-- End of file: ./999_seed/01_seed_tenants.sql ---------------------------

-- Start of file: ./999_seed/03_seed_companies.sql -------------------------

INSERT INTO "company" (name, attributes, tenant_id) VALUES
('Company One', '{}', 1),
('Company Two', '{}', 2),
('Company Three', '{}', 3)
ON CONFLICT DO NOTHING;
-- End of file: ./999_seed/03_seed_companies.sql ---------------------------

-- Start of file: ./999_seed/04_seed_customers.sql -------------------------

INSERT INTO "customer" (name, attributes, tenant_id) VALUES
('Customer One', '{}', 1),
('Customer Two', '{}', 2),
('Customer Three', '{}', 3)
ON CONFLICT DO NOTHING;
-- End of file: ./999_seed/04_seed_customers.sql ---------------------------

-- Start of file: ./999_seed/05_seed_relationships.sql -------------------------

INSERT INTO "relationships" (customer_id, entity, entity_id) VALUES
(1, 'company', 1),
(2, 'company', 2),
(3, 'company', 3)
ON CONFLICT DO NOTHING;
-- End of file: ./999_seed/05_seed_relationships.sql ---------------------------

-- Start of file: ./999_seed/99_seed_users.sql -------------------------

INSERT INTO "app_user" (username, name, email, password_hash, salt, role, enabled, tenant_id) VALUES
('user1', 'User One', 'user1@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 1),
('user1', 'Admin One', 'admin1@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 1),
('user2', 'User Two', 'user2@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 2),
('user1', 'Admin Two', 'admin2@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 2),
('user3', 'User Three', 'user3@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'standard', true, 3),
('user1', 'Admin Three', 'admin3@example.com', 'o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 'g8eY0NC2wVYWA2y4chGP7Q==', 'admin', true, 3)
ON CONFLICT DO NOTHING;
-- End of file: ./999_seed/99_seed_users.sql ---------------------------

-- Start of file: ./99_xtn/01_add_metadata_columns.sql -------------------------

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
            ADD COLUMN IF NOT EXISTS updated_by_id INT NOT NULL DEFAULT 0 REFERENCES app_user(id) ON DELETE SET DEFAULT;
        ', table_name);
    END LOOP;
END $$;
-- End of file: ./99_xtn/01_add_metadata_columns.sql ---------------------------

-- Start of file: ./99_xtn/02_update_ts_fn.sql -------------------------

CREATE OR REPLACE FUNCTION update_updated_ts()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_ts = EXTRACT(EPOCH FROM NOW());
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
-- End of file: ./99_xtn/02_update_ts_fn.sql ---------------------------

-- Start of file: ./99_xtn/03_add_update_ts_triggers.sql -------------------------

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
-- End of file: ./99_xtn/03_add_update_ts_triggers.sql ---------------------------

