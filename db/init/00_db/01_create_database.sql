DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'miniflexcrm') THEN
        CREATE DATABASE miniflexcrm;
    END IF;
END $$;

\c miniflexcrm

