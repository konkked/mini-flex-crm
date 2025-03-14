CREATE INDEX IF NOT EXISTS idx_note_tenantId_userId_route
ON note (tenant_id, user_id, route);