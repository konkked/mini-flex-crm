CREATE TABLE IF NOT EXISTS "team" (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id),
    owner_id INT NOT NULL REFERENCES app_user(id),
    attributes JSON NULL
);

CREATE TABLE IF NOT EXISTS "team_member" (
    id SERIAL PRIMARY KEY,
    team_member_type team_member_type NOT NULL,
    team_id INT NOT NULL REFERENCES team(id),
    member_id INT NOT NULL REFERENCES app_user(id)
)

CREATE TABLE IF NOT EXISTS "team_account" (
    id SERIAL PRIMARY KEY,
    team_id INT NOT NULL REFERENCES team(id),
    account_id INT NOT NULL REFERENCES account(id)
)