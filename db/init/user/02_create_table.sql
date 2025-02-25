CREATE TYPE user_role AS ENUM ('admin', 'standard', 'guest');

CREATE TABLE IF NOT EXISTS "user" (
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


INSERT INTO "user" (username, name, email, password_hash, salt, role, enabled, tenant_id)
VALUES ('super_admin', 'Darth Keyser', '1chkeyser1@gmail.com','XVuteO1ArA3PrNTC4szULSJzuBR+yrpFgnSFbVrprkw=', 
    'g8eY0NC2wVYWA2y4chGP7Q==','admin', true, 0);