
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM "app_user" WHERE username = 'super_admin') THEN
            INSERT INTO "app_user" (id, username, name, email, password_hash, salt, role, enabled, tenant_id)
            VALUES (0, 'super_admin', 'Darth Keyser', '1chkeyser1@gmail.com','o2mAD9hQCuAeFClTb09w7t2efPKfNTxAgQrpRyNURAI=', 
                        'g8eY0NC2wVYWA2y4chGP7Q==','admin', true, 0);
            RAISE NOTICE 'Super admin created';
        END IF;
END $$;
