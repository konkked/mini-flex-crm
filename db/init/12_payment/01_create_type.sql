DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'payment_type') THEN
        CREATE TYPE payment_type AS ENUM ('Card','Cash','Charge-Off');
    END IF;
END $$;