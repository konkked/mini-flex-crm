DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'support_ticket_status_type') THEN
        CREATE TYPE support_ticket_status_type AS ENUM ('open','review','rejected','fixed','closed');
    END IF;
END $$;
