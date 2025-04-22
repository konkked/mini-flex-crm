DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'deal_status_type') THEN
        CREATE TYPE lead_status_type AS ENUM ('Abandoned','Qualified','Outreach','Nurture','Closing','Closed');
    END IF;
END $$;