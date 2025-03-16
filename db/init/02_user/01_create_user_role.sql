DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'app_user_role') THEN
        CREATE TYPE app_user_role AS ENUM ('admin','group_manager','manager','pre-sales','sales','guest');
    END IF;
END $$;
