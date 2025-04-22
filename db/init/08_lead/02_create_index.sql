CREATE INDEX IF NOT EXISTS idx_lead_tenant_id
ON lead (tenant_id);

CREATE INDEX IF NOT EXISTS idx_lead_owner_id
ON lead (owner_id);