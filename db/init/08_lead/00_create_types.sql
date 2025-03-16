DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'lead_quality_type') THEN
        CREATE TYPE lead_status_type AS ENUM ('Dead','Raw','Bronze','Silver','Gold','Qualified');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'lead_data_origin_type') THEN
        CREATE TYPE lead_data_origin_type AS ENUM ('Scraped','Manual','Bulk Entry','Import');
    END IF;
END $$;
