DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'team_member_type') THEN
        CREATE TYPE team_member_type AS ENUM ('pre_sales','sales','account_manager', 'support');
    END IF;
END $$;
