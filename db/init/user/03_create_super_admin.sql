
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM "user" WHERE username = 'super_admin') THEN
            INSERT INTO "user" (id, username, [name], email, password_hash, salt, [role], [enabled], tenant_id)
            VALUES (0, 'super_admin', 'Darth Keyser', '1chkeyser1@gmail.com','XVuteO1ArA3PrNTC4szULSJzuBR+yrpFgnSFbVrprkw=', 
                        'g8eY0NC2wVYWA2y4chGP7Q==','admin', true, 0);
        END IF;
END $$;
