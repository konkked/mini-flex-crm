CREATE INDEX IF NOT EXISTS idx_attachment_tenantId_path
ON support_ticket (tenant_id, path);