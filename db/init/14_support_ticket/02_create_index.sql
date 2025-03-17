CREATE INDEX IF NOT EXISTS idx_support_ticket_customer_id
ON support_ticket (user_id, tenant_id);