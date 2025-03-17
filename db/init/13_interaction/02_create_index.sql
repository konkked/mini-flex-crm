CREATE INDEX IF NOT EXISTS idx_interaction_tenant_id
ON interaction (tenant_id, customer_id, contact_id, lead_id);