CREATE INDEX IF NOT EXISTS idx_deal_tenant_id
ON deal(tenant_id);

CREATE INDEX IF NOT EXISTS idx_deal_owner_id
ON deal(owner_id);